using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// A factory that provides valid instances of language detection (cognitive) services powered by Text Analylitics from Azure Cognitive Services for Language.
/// </summary>
internal class TextAnalyticsLanguageDetectionServiceFactory : CognitiveServiceFactoryBase<TextAnalyticsLanguageDetectionService, TextAnalyticsLanguageDetectionServiceOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextAnalyticsLanguageDetectionServiceFactory"/> class.
    /// </summary>
    /// <param name="configurations">Configurations for cognitive services.</param>
    public TextAnalyticsLanguageDetectionServiceFactory(IOptions<TextAnalyticsLanguageDetectionConfigurations> configurations)
        : base(configurations, options => new TextAnalyticsLanguageDetectionService(options))
    {
    }
}
