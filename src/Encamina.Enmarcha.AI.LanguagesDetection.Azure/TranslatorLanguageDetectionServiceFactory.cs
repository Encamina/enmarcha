using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// A factory that provides valid instances of language detection (cognitive) services powered by Azure Translator.
/// </summary>
internal class TranslatorLanguageDetectionServiceFactory : CognitiveServiceFactoryBase<TranslatorLanguageDetectionService, TranslatorLanguageDetectionServiceOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionServiceFactory"/> class.
    /// </summary>
    /// <param name="configurations">Configurations for cognitive services.</param>
    /// <param name="httpClientFactory">An <see cref="HttpClient"/> factory required for calls to the Azure Translation sevice.</param>
    public TranslatorLanguageDetectionServiceFactory(IOptions<TranslatorLanguageDetectionConfigurations> configurations, IHttpClientFactory httpClientFactory)
        : base(configurations, options => new TranslatorLanguageDetectionService(options, httpClientFactory))
    {
    }
}
