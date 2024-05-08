using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Encamina.Enmarcha.Bot.Abstractions.Dialogs;
using Encamina.Enmarcha.Bot.Abstractions.Extensions;

using Encamina.Enmarcha.Core.Extensions;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using ResponseMessages = Encamina.Enmarcha.Bot.Skills.QuestionAnswering.Resources.ResponseMessages;

namespace Encamina.Enmarcha.Bot.Skills.QuestionAnswering;

/// <summary>
/// The dialog for the question answering skill.
/// </summary>
internal sealed class QuestionAnsweringDialog : NamedDialogBase, IIntendable
{
    private readonly QuestionAnsweringSkillOptions configurationOptions;
    private readonly QuestionAnsweringDialogServices services;

    private readonly bool isMetadataProcessorAvailable;
    private readonly bool isSourcesProcessorAvailable;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringDialog"/> class.
    /// </summary>
    /// <param name="id">Unique identifier for this dialog.</param>
    /// <param name="optionsSnapshot">Configuration for this question answering dialog.</param>
    /// <param name="questionAnsweringDialogServices">Services required for this question answering dialog.</param>
    public QuestionAnsweringDialog(string id, QuestionAnsweringDialogServices questionAnsweringDialogServices, IOptionsMonitor<QuestionAnsweringSkillOptions> optionsSnapshot)
        : base($@"{nameof(QuestionAnsweringDialog)}-{id}")
    {
        configurationOptions = optionsSnapshot.Get(id);

        services = questionAnsweringDialogServices;

        if (services.BotTelemetryClient != null)
        {
            TelemetryClient = services.BotTelemetryClient;
        }

        isMetadataProcessorAvailable = services.MetadataProcessor != null;
        isSourcesProcessorAvailable = services.SourcesProcessor != null;
    }

    /// <inheritdoc/>
    public string Intent => string.IsNullOrWhiteSpace(configurationOptions.DialogIntent) ? Constants.DefaultIntent : configurationOptions.DialogIntent;

    /// <inheritdoc/>
    public override string Name => string.IsNullOrWhiteSpace(configurationOptions.DialogName) ? base.Name : configurationOptions.DialogName;

    /// <inheritdoc/>
    public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
    {
        ResourceResponse? response = null;

        if (dc.Context.Activity.Type == ActivityTypes.Message && !string.IsNullOrWhiteSpace(dc.Context.Activity.Text))
        {
            var message = dc.Context.Activity.Text;

            var question = configurationOptions.NormalizeMessage
                ? message.NormalizeDiacritics(configurationOptions.NormalizeRemoveCharacters, configurationOptions.NormalizeReplaceCharacters.ToDictionary(item => item.Key[0], item => item.Value[0]))
                : message;

            var metadataOptions = isMetadataProcessorAvailable ? await services.MetadataProcessor.ProcessMessageAsync(question, dc.Context.TurnState.Get<MetadataOptions>(), cancellationToken) : default;

            var sources = isSourcesProcessorAvailable ? await services.SourcesProcessor.ProcessMessageAsync(question, cancellationToken) : Enumerable.Empty<string>();

            var request = await BuildQuestionRequestAsync(question, dc.Context.Activity.From.Id, metadataOptions, sources, cancellationToken);

            var result = await services.QuestionAnsweringCognitiveServiceFactory.GetByName(configurationOptions.QuestionAnsweringServiceName).GetAnswersAsync(request, cancellationToken);

            response = await SendAnswerAsync(await ProcessQuestionResultAsync(result, metadataOptions, sources, cancellationToken), dc.Context, cancellationToken);
        }

        return await dc.EndDialogAsync(response, cancellationToken);
    }

    private static IActivity BuildMessageActivity(string message, bool withVerbose, IEnumerable<IAnswer>? answers)
    {
        var activity = MessageFactory.Text(message, message);

        if (withVerbose)
        {
            activity.Properties = JObject.FromObject(new { Verbose = new { Answers = answers } });
        }

        return activity;
    }

    private static bool VerboseFromTurnContextValue(ITurnContext turnContext)
    {
        return turnContext.Activity.Value is JObject activityValue && (activityValue[Constants.ActivityValueVerbose]?.Value<bool>() ?? false);
    }

    private async Task<QuestionRequest> BuildQuestionRequestAsync(string question, string userId, MetadataOptions? metadataOptions, IEnumerable<string> sources, CancellationToken cancellationToken)
    {
        var questionRequestOptions = new QuestionRequestOptions(sources)
        {
            MetadataOptions = metadataOptions,
        };

        return services.QuestionRequestProcessor != null
            ? await services.QuestionRequestProcessor.ProcessAsync<QuestionRequest>(question, userId, questionRequestOptions, cancellationToken)
            : new QuestionRequest()
            {
                Options = questionRequestOptions,
                Question = question,
                UserId = userId,
            };
    }

    private async Task<IEnumerable<IAnswer>> ProcessQuestionResultAsync(QuestionResult result, MetadataOptions? metadataOptions, IEnumerable<string> sources, CancellationToken cancellationToken)
    {
        IEnumerable<IAnswer> answers = result.Answers;

        if (services.QuestionResultProcessor != null)
        {
            answers = await services.QuestionResultProcessor.ProcessAsync(result, cancellationToken);
        }

        if (isSourcesProcessorAvailable)
        {
            answers = await services.SourcesProcessor.ProcessAnswersAsync(answers, sources, cancellationToken);
        }

        if (isMetadataProcessorAvailable)
        {
            answers = await services.MetadataProcessor.ProcessAnswersAsync(answers, metadataOptions, cancellationToken);
        }

        return answers;
    }

    private async Task<ResourceResponse?> SendAnswerAsync(IEnumerable<IAnswer> answers, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var verbose = configurationOptions.Verbose || VerboseFromTurnContextValue(turnContext);

        if (answers?.Any() ?? false)
        {
            if (services.SendAnswersProcessor != null)
            {
                var result = await services.SendAnswersProcessor.SendResponseAsync(turnContext, answers, verbose, cancellationToken);

                if (result.Successful)
                {
                    return result.ResourceResponse;
                }
            }

            if (configurationOptions.FallbackToFirstAnswer)
            {
                return await turnContext.SendActivityAsync(BuildMessageActivity(answers.First().Value, verbose, answers), cancellationToken);
            }
        }

        if (!turnContext.Responded && services.IntentResponsesProvider != null)
        {
            var confusedResponses = await services.IntentResponsesProvider.GetResponsesAsync(Constants.ConfusedIntent, turnContext.Activity.GetCultureInfoFromActivity(), cancellationToken);

            if (confusedResponses.Any())
            {
                return (await turnContext.SendActivitiesAsync(confusedResponses.Select(response => BuildMessageActivity(response.Text, verbose, answers)).ToArray(), cancellationToken)).Last();
            }
        }

        return await turnContext.SendActivityAsync(BuildMessageActivity(ResponseMessages.ResourceManager.GetStringByCurrentCulture(nameof(ResponseMessages.ErrorConfused)), verbose, answers), cancellationToken);
    }
}
