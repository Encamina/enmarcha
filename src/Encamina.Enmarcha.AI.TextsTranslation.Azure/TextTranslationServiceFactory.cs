using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.TextsTranslation.Azure;

/// <summary>
/// A factory that provides valid instances of text translation (cognitive) services.
/// </summary>
internal class TextTranslationServiceFactory : CognitiveServiceFactoryBase<TextTranslationService, TextTranslationServiceOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextTranslationServiceFactory"/> class.
    /// </summary>
    /// <param name="configurations">Configurations for cognitive services.</param>
    /// <param name="httpClientFactory">An <see cref="HttpClient"/> factory required for calls to the Azure Translation sevice.</param>
    /// <param name="normalizers">
    /// A collection of normalizers to fix or change results from text translations to overcome or correct any discrepancy or unexpected result.
    /// </param>
    public TextTranslationServiceFactory(IOptions<TextTranslationConfigurations> configurations, IHttpClientFactory httpClientFactory, IEnumerable<ITextTranslationNormalizer> normalizers)
        : base(configurations, options => new TextTranslationService(options, httpClientFactory, normalizers))
    {
    }
}
