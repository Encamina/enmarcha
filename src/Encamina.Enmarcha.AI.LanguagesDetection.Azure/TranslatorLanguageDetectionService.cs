using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json.Serialization;

using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions.Extensions;
using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// A language detection service powered by the Text Analytics feature
/// from Azure Cognitive Service for Language service.
/// </summary>
internal class TranslatorLanguageDetectionService : CognitiveServiceBase<TranslatorLanguageDetectionServiceOptions>, ILanguageDetectionService
{
    private const string SubscriptionKey = @"Ocp-Apim-Subscription-Key";
    private const string SubscriptionRegion = @"Ocp-Apim-Subscription-Region";
    private const string ServicePath = @"detect";
    private const string VersionQueryParameter = @"api-version=3.0";

    private readonly IHttpClientFactory httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatorLanguageDetectionService"/> class.
    /// </summary>
    /// <param name="options">Options for this text translation service.</param>
    /// <param name="httpClientFactory">An <see cref="HttpClient"/> factory required for calls to the Azure Translation sevice.</param>
    public TranslatorLanguageDetectionService(TranslatorLanguageDetectionServiceOptions options, IHttpClientFactory httpClientFactory) : base(options)
    {
        this.httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task<LanguageDetectionResult> DetectLanguageAsync(LanguageDetectionRequest request, CancellationToken cancellationToken)
    {
        request.Validate();

        var uriBuilder = new UriBuilder(Options.EndpointUrl.AbsolutePath.EndsWith(ServicePath, StringComparison.OrdinalIgnoreCase) ? Options.EndpointUrl : Options.EndpointUrl.Append(ServicePath))
        {
            Query = VersionQueryParameter,
        };

        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add(SubscriptionKey, Options.KeyCredential);
        httpClient.DefaultRequestHeaders.Add(SubscriptionRegion, Options.RegionName);

        var response = await httpClient.PostAsync(uriBuilder.Uri,
                                                  JsonContent.Create(request.Text.Select(t => new { t.Id, Text = t.Value }), new MediaTypeHeaderValue(MediaTypeNames.Application.Json)),
                                                  cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var languageDetectionResponses = await response.Content.ReadFromJsonAsync<IEnumerable<TranslationLanguageDetectionResponse>>(cancellationToken: cancellationToken);

            if (Options.DetectOnlyTranslatableLanguages)
            {
                languageDetectionResponses = languageDetectionResponses.Where(item => item.IsTranslationSupported);
            }

            return new LanguageDetectionResult()
            {
                DetectedLanguages = languageDetectionResponses.Where(item => item.ConfidenceScore >= (Options.ConfidenceThreshold ?? 0)).Select((item, index) => new DetectedLanguage()
                {
                    Id = request.Text.ElementAt(index).Id,
                    ConfidenceScore = item.ConfidenceScore,
                    Language = CultureInfo.GetCultureInfo(item.Language),
                }).ToList(), // This 'ToList' call prevents multitple enumerations, improving performance.
            };
        }

        throw new TranslatorLanguageDetectionServiceException($@"Error translating texts. Code was '{response.StatusCode}' and message '{await response.Content.ReadAsStringAsync()}'");
    }

    private sealed class TranslationLanguageDetectionResponse
    {
        [JsonPropertyName(@"language")]
        public string Language { get; set; }

        [JsonPropertyName(@"score")]
        public double ConfidenceScore { get; set; }

        [JsonPropertyName(@"isTranslationSupported")]
        public bool IsTranslationSupported { get; set; }
    }
}
