using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Azure;

/// <summary>
/// A text translation service powered by the Azure Translator service.
/// </summary>
/// <remarks>
/// <para>
/// Currently, for single text translations, the Azure Translator service does not provides an SDK; instead
/// it offers a REST API. Therefore, this service implementation does direct HTTP calles to the Azure
/// Translator REST API using parameters provided by options.
/// </para>
/// <para>
/// This services uses Azure Translator v3.0.
/// </para>
/// </remarks>
/// <seealso href="https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-reference"/>
internal class TextTranslationService : CognitiveServiceBase<TextTranslationServiceOptions>, ITextTranslationService
{
    private const string SubscriptionKey = @"Ocp-Apim-Subscription-Key";
    private const string SubscriptionRegion = @"Ocp-Apim-Subscription-Region";
    private const string ServicePath = @"translate";
    private const string VersionQueryParameter = @"api-version=3.0";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IEnumerable<ITextTranslationNormalizer> normalizers;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextTranslationService"/> class.
    /// </summary>
    /// <param name="options">Options for this text translation service.</param>
    /// <param name="httpClientFactory">An <see cref="HttpClient"/> factory required for calls to the Azure Translation sevice.</param>
    /// <param name="normalizers">
    /// A collection of normalizers to fix or change results from text translations to overcome or correct any discrepancy or unexpected result.
    /// </param>
    public TextTranslationService(TextTranslationServiceOptions options, IHttpClientFactory httpClientFactory, IEnumerable<ITextTranslationNormalizer> normalizers) : base(options)
    {
        this.httpClientFactory = httpClientFactory;
        this.normalizers = normalizers?.OrderBy(n => n.Order) ?? Enumerable.Empty<ITextTranslationNormalizer>();
    }

    /// <inheritdoc/>
    public async Task<TextTranslationResult> TranslateAsync(TextTranslationRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var uriBuilder = new UriBuilder(Options.EndpointUrl.AbsolutePath.EndsWith(ServicePath, StringComparison.OrdinalIgnoreCase) ? Options.EndpointUrl : Options.EndpointUrl.Append(ServicePath))
        {
            Query = BuildQueryParameters(request),
        };

        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add(SubscriptionKey, Options.KeyCredential);
        httpClient.DefaultRequestHeaders.Add(SubscriptionRegion, Options.RegionName);

        var response = await httpClient.PostAsync(uriBuilder.Uri,
                                                  JsonContent.Create(request.Texts.Select(t => new { Id = t.Key, Text = t.Value }), new MediaTypeHeaderValue(MediaTypeNames.Application.Json)),
                                                  cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var textTranslationResponses = await response.Content.ReadFromJsonAsync<IEnumerable<TextTranslationResponse>>(cancellationToken: cancellationToken);

            return new TextTranslationResult()
            {
                TextTranslations = textTranslationResponses.Select((t, i) =>
                {
                    var current = request.Texts.ElementAt(i);

                    return new TextTranslation()
                    {
                        Id = current.Key,
                        Value = current.Value,
                        ConfidenceScore = t.DetectedLanguage?.ConfidenceScore,
                        Translations = t.Translation.ToDictionary(e => e.Language.Name, e => Normalize(e.Text)),
                    };
                }).ToList(), // This 'ToList' call prevents multitple enumerations, improving performance.
            };
        }

        throw new TextTranslationServiceException($@"Error translating texts. Code was '{response.StatusCode}' and message '{await response.Content.ReadAsStringAsync()}'");
    }

    private static string BuildQueryParameters(TextTranslationRequest request)
    {
        var result = new StringBuilder(VersionQueryParameter);

        foreach (var languageCode in request.ToLanguages.Select(l => l.Name))
        {
            result.Append($@"&to={languageCode}");
        }

        // The 'from' parameter is optional.
        if (!string.IsNullOrWhiteSpace(request.FromLanguage?.Name))
        {
            result.Append($@"&from={request.FromLanguage.Name}");
        }

        return result.ToString();
    }

    private static void ValidateRequest(TextTranslationRequest request)
    {
        Guard.IsNotNull(request);
        Guard.IsNotNull(request.ToLanguages);
        Guard.IsNotEmpty(request.ToLanguages);
        Guard.IsNotNull(request.Texts);
        Guard.IsNotEmpty(request.Texts);

        Guard.IsTrue(request.Texts.Keys.All(k => !string.IsNullOrWhiteSpace(k)), nameof(request.Texts), Resources.ExceptionMessages.RequestParameterWithInvalidIdentifier);
        Guard.IsTrue(request.Texts.Keys.Distinct().Count() == request.Texts.Keys.Count, nameof(request.Texts), Resources.ExceptionMessages.RequestParameterWithRepeatedIdentifier);
    }

    private string Normalize(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            foreach (var normalizer in normalizers)
            {
                value = normalizer.Normalize(value);
            }
        }

        return value;
    }

    private sealed class TextTranslationResponse
    {
        [JsonPropertyName(@"detectedLanguage")]
        public DetectedLanguage DetectedLanguage { get; set; }

        [JsonPropertyName(@"translations")]
        public IEnumerable<Translation> Translation { get; set; }
    }

    private sealed class DetectedLanguage
    {
        [JsonPropertyName(@"language")]
        public string Language { get; set; }

        [JsonPropertyName(@"score")]
        public double? ConfidenceScore { get; set; }
    }

    private sealed class Translation
    {
        [JsonPropertyName(@"text")]
        public string Text { get; set; }

        [JsonPropertyName(@"to")]
        public string To { get; set; }

        public CultureInfo Language => new(To);
    }
}
