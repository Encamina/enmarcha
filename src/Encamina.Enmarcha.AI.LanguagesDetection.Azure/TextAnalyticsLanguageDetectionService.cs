using System.Globalization;

using Azure;
using Azure.AI.TextAnalytics;

using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions.Extensions;

using DetectedLanguage = Encamina.Enmarcha.AI.LanguagesDetection.Abstractions.DetectedLanguage;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

/// <summary>
/// A language detection service powered by the Text Analytics feature
/// from Azure Cognitive Service for Language service.
/// </summary>
internal class TextAnalyticsLanguageDetectionService : CognitiveServiceBase<TextAnalyticsLanguageDetectionServiceOptions>, ILanguageDetectionService
{
    private const string CountryHintParameter = @"CountryHint";

    private readonly TextAnalyticsClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextAnalyticsLanguageDetectionService"/> class.
    /// </summary>
    /// <param name="options">Options for this question answering service.</param>
    public TextAnalyticsLanguageDetectionService(TextAnalyticsLanguageDetectionServiceOptions options) : base(options)
    {
        Options = options;

        client = new TextAnalyticsClient(options.EndpointUrl, new AzureKeyCredential(options.KeyCredential));
    }

    /// <inheritdoc/>
    public async Task<LanguageDetectionResult> DetectLanguageAsync(LanguageDetectionRequest request, CancellationToken cancellationToken)
    {
        request.Validate();

        var options = new TextAnalyticsRequestOptions()
        {
            IncludeStatistics = Options.IncludeStatistics,
        };

        var result = await client.DetectLanguageBatchAsync(request.Text.Select(t => new DetectLanguageInput(t.Id, t.Value)
        {
            CountryHint = request.AdditionalParameters.TryGetValue(CountryHintParameter, out var countryHint) ? countryHint : DetectLanguageInput.None,
        }), options, cancellationToken);

        return new LanguageDetectionResult()
        {
            DetectedLanguages = result.Value.Where(v => v.PrimaryLanguage.ConfidenceScore >= (Options.ConfidenceThreshold ?? 0)).Where(t => !t.HasError).Select(v => new DetectedLanguage()
            {
                Id = v.Id,
                Language = CultureInfo.GetCultureInfo(v.PrimaryLanguage.Iso6391Name.Replace('_', '-')), // Sometimes, the response returns '_' instead of '-', like for example with Chinese.
                ConfidenceScore = v.PrimaryLanguage.ConfidenceScore,
            }),
        };
    }
}
