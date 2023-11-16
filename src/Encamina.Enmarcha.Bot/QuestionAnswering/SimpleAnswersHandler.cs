using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Encamina.Enmarcha.Bot.Abstractions.QuestionAnswering;

using Microsoft.Bot.Builder;

using Newtonsoft.Json.Linq;

namespace Encamina.Enmarcha.Bot.QuestionAnswering;

/// <summary>
/// A simple answer handler that sends the first answer found with the highest confidence score as the response.
/// </summary>
public class SimpleAnswersHandler : ISendAnswersHandler
{
    /// <inheritdoc/>
    public virtual int Order => int.MaxValue; // Ensure this is the last response handler to be evaluated.

    /// <inheritdoc/>
    public virtual Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken)
        where TAnswer : IAnswer => HandleSendResponseAsync(context, answers, false, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, bool withVerbose, CancellationToken cancellationToken)
        where TAnswer : IAnswer
    {
        if (context != null && (answers?.Any() ?? false))
        {
            var highestConfidenceScoredAnswer = answers.OrderByDescending(a => a.ConfidenceScore).First().Value;
            var activity = MessageFactory.Text(highestConfidenceScoredAnswer, highestConfidenceScoredAnswer);

            if (withVerbose)
            {
                activity.Properties = BuildVerboseInformation(answers);
            }

            return new SendResponseResult(await context.SendActivityAsync(activity, cancellationToken: cancellationToken));
        }

        return SendResponseResult.Empty;
    }

    /// <summary>
    /// Builds verbose information to add into an activity.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the answers.</typeparam>
    /// <param name="answers">A collection of answers that can be used to build verbose information.</param>
    /// <returns>A JSON object (<see cref="JObject"/>) with the built verbose information.</returns>
    protected virtual JObject BuildVerboseInformation<TAnswer>(IEnumerable<TAnswer> answers) where TAnswer : IAnswer
    {
        return JObject.FromObject(new { Verbose = new { Answers = answers } });
    }
}
