using Azure;
using Azure.AI.OpenAI;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.OpenAI.Azure;

internal sealed class CompletionService : ICompletionService
{
    private CompletionServiceOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionService"/> class.
    /// </summary>
    /// <param name="options">Current options for this completion service.</param>
    public CompletionService(IOptionsMonitor<CompletionServiceOptions> options)
    {
        this.options = options.CurrentValue;

        options.OnChange(newOptions => this.options = newOptions);
    }

    /// <inheritdoc/>
    public string Name => options.Name;

    /// <inheritdoc/>
    public string Id => options.Id;

    /// <inheritdoc/>
    object IIdentifiable.Id => Id;

    /// <inheritdoc/>
    public async Task<CompletionResult> CompleteAsync(CompletionRequest request, CancellationToken cancellationToken)
    {
        var client = new OpenAIClient(options.EndpointUrl, new AzureKeyCredential(options.KeyCredential), options);

        var completionsOptions = new CompletionsOptions()
        {
            ChoicesPerPrompt = request.NumberOfCompletionsPerPrompt,
            DeploymentName = options.DeploymentName,
            Echo = request.DoEcho,
            FrequencyPenalty = request.FrequencyPenalty,
            GenerationSampleCount = request.BestOf,
            MaxTokens = request.MaxTokens,
            NucleusSamplingFactor = request.TopProbability,
            PresencePenalty = request.PresencePenalty,
            Temperature = request.Temperature,
            User = request.UserId,
        };

        completionsOptions.Prompts.AddRange(request.Prompts);
        completionsOptions.StopSequences.AddRange(request.StopSequences);

        var response = (await client.GetCompletionsAsync(completionsOptions, cancellationToken)).Value; // Any error while calling Azure OpenAI is handled and thrown by the `GetCompletionsAsync` method itself...

        return new CompletionResult()
        {
            Id = response.Id,
            CreatedUtc = response.Created.UtcDateTime,
            Completitions = response.Choices.Select(choice => new Completition()
            {
                Text = choice.Text,
                Index = choice.Index,
                FinishReason = choice.FinishReason.ToString(),
            }),
        };
    }
}
