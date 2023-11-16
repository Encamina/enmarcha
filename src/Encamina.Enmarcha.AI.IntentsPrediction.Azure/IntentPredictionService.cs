using System.Collections.ObjectModel;
using System.Text.Json;

using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// An intention prediction service powered by the Conversation Analysis feature
/// from Azure Cognitive Service for Language service.
/// </summary>
internal class IntentPredictionService : CognitiveServiceBase<IntentPredictionServiceOptions>, IIntentPredictionService
{
    private static readonly IDictionary<string, Func<string, JsonElement, IIntentResult>> Handlers = new Dictionary<string, Func<string, JsonElement, IIntentResult>>(StringComparer.OrdinalIgnoreCase)
    {
        { @"Conversation", BuildConversationAnalysisIntentResult },
        { @"QuestionAnswering", BuildQuestionAnsweringTargetIntentResult },
    };

    private readonly ConversationAnalysisClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntentPredictionService"/> class.
    /// </summary>
    /// <param name="options">Options for this intention prediction service.</param>
    public IntentPredictionService(IntentPredictionServiceOptions options) : base(options)
    {
        Options = options;

        client = new ConversationAnalysisClient(options.EndpointUrl, new AzureKeyCredential(options.KeyCredential));
    }

    /// <inheritdoc/>
    public async Task<IntentPredictionResult> PredictIntentAsync(IntentPredictionRequest request, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(request);
        Guard.IsNotNullOrWhiteSpace(request.Utterance);

        var data = new
        {
            analysisInput = new
            {
                conversationItem = new
                {
                    text = request.Utterance,
                    id = request.Id,
                    participantId = request.ParticipantId,
                    language = request.IntentPredictionOptions?.Language?.Name ?? string.Empty,
                }
            },
            parameters = new
            {
                projectName = Options.OrchestratorName,
                deploymentName = Options.DeploymentSlot,
                verbose = Options.EnableVerbose,
                stringIndexType = @"Utf16CodeUnit", // Use Utf16CodeUnit for strings in .NET.
            },
            kind = @"Conversation",
        };

        var requestContext = new RequestContext()
        {
            CancellationToken = cancellationToken,
            ErrorOptions = ErrorOptions.Default,
        };

        var response = await client.AnalyzeConversationAsync(RequestContent.Create(data), requestContext);

        using var result = await JsonDocument.ParseAsync(response.ContentStream, cancellationToken: cancellationToken);
        var prediction = result.RootElement.GetProperty(@"result").GetProperty(@"prediction");
        var topIntent = prediction.GetProperty(@"topIntent").GetString();

        return new IntentPredictionResult()
        {
            IntentPrediction = new IntentPrediction()
            {
                TopIntentName = topIntent,
                Intents = BuildIntents(prediction),
            },
        };
    }

    private static IReadOnlyDictionary<string, IIntentResult> BuildIntents(JsonElement prediction)
    {
        var values = prediction.GetProperty(@"intents")
                               .EnumerateObject()
                               .Select(i => Handlers.TryGetValue(i.Value.GetProperty(@"targetProjectKind").GetString(), out var handler) ? handler(i.Name, i.Value) : null)
                               .Where(i => i != null)
                               .ToDictionary(i => i.Name);

        return new ReadOnlyDictionary<string, IIntentResult>(values ?? new Dictionary<string, IIntentResult>());
    }

    private static ConversationAnalysisIntentResult BuildConversationAnalysisIntentResult(string name, JsonElement intent)
    {
        // TODO - After the change to Azure.AI.Language.Conversations version 1.0.0, the complexity to
        // translate from JSON response to ConversationAnalysisIntentResult is too high.
        throw new NotImplementedException();
    }

    private static QuestionAnsweringIntentResult BuildQuestionAnsweringTargetIntentResult(string name, JsonElement intent)
    {
        return new QuestionAnsweringIntentResult()
        {
            ConfidenceScore = intent.GetProperty(@"confidenceScore").GetDouble(),
            Name = name,
            Answers = intent.GetProperty(@"result").GetProperty(@"answers").EnumerateArray().Select(a => new Answer()
            {
                AssociatedQuestions = a.GetProperty(@"questions").EnumerateArray().Select(i => i.GetString()).ToArray(),
                ConfidenceScore = a.GetProperty(@"confidenceScore").GetDouble(),
                Id = a.GetProperty(@"id").GetString(),
                Metadata = JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(a.GetProperty(@"metadata")),
                Source = a.TryGetProperty(@"source", out var sourceProperty) ? sourceProperty.GetString() : null, // Source is the only property that is not returned when the answer is the "I dont't know" answer (Id = -1).
                Value = a.GetProperty(@"answer").GetString(),
            }).ToArray(),
        };
    }
}
