using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

internal record TextAnalyticsLanguageDetectionServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Gets a value indicating whether the service should return statistics with the
    /// results of the operation. Default is <see langword="false"/>.
    /// </summary>
    public bool IncludeStatistics { get; init; } = false;

    /// <summary>
    /// Gets minimum confidence threshold score for language detection, value ranges from 0 to 1.
    /// </summary>
    public double? ConfidenceThreshold { get; init; }
}
