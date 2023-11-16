using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// Configurations for intent prediction services.
/// </summary>
internal record IntentPredictionConfigurations : ICognitiveServiceConfigurationsBase<IntentPredictionServiceOptions>
{
    /// <summary>
    /// Gets the collection of specific intent prediction service options in this configuration.
    /// </summary>
    public IReadOnlyList<IntentPredictionServiceOptions> IntentPredictionOptions { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<IntentPredictionServiceOptions> CognitiveServiceOptions => IntentPredictionOptions;
}
