using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Azure;

/// <summary>
/// Configurations for text translation services.
/// </summary>
internal record TextTranslationConfigurations : ICognitiveServiceConfigurationsBase<TextTranslationServiceOptions>
{
    /// <summary>
    /// Gets the collection of specific text translation service options in this configuration.
    /// </summary>
    public IReadOnlyList<TextTranslationServiceOptions> TextTranslationOptions { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<TextTranslationServiceOptions> CognitiveServiceOptions => TextTranslationOptions;
}
