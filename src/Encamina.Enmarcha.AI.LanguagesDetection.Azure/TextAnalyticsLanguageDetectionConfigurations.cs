using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// Configurations for language detection services powered by the language detection feature of Text Analytics from Azure Cognitive Service for Language.
/// </summary>
internal record TextAnalyticsLanguageDetectionConfigurations : ICognitiveServiceConfigurationsBase<TextAnalyticsLanguageDetectionServiceOptions>
{
    /// <summary>
    /// Gets the collection of specific language detection service options in this configuration.
    /// </summary>
    public IReadOnlyList<TextAnalyticsLanguageDetectionServiceOptions> TextAnalyticsLanguageDetectionServiceOptions { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<TextAnalyticsLanguageDetectionServiceOptions> CognitiveServiceOptions => TextAnalyticsLanguageDetectionServiceOptions;
}
