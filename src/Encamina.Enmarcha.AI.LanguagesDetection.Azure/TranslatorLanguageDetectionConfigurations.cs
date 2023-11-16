using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

internal record TranslatorLanguageDetectionConfigurations : ICognitiveServiceConfigurationsBase<TranslatorLanguageDetectionServiceOptions>
{
    /// <summary>
    /// Gets the collection of specific language detection service options in this configuration.
    /// </summary>
    public IReadOnlyList<TranslatorLanguageDetectionServiceOptions> TranslatorLanguageDetectionServiceOptions { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<TranslatorLanguageDetectionServiceOptions> CognitiveServiceOptions => TranslatorLanguageDetectionServiceOptions;
}
