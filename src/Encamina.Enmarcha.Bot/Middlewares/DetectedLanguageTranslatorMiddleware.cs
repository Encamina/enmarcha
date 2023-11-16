using System.Globalization;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Schema;

#pragma warning disable S103 // Lines should not be too long

namespace Encamina.Enmarcha.Bot.Middlewares;

/// <summary>
/// Middleware to automatically translate from a detected language to a configure translation language.
/// </summary>
public class DetectedLanguageTranslatorMiddleware : IMiddleware
{
    private const string DefaultRequestId = "0"; // Use zero (0) as default ID for translation services that only provides one single value in the request.

    private readonly CultureInfo translateToLanguage;
    private readonly IEnumerable<CultureInfo> languageExceptions;
    private readonly ITextTranslationService translationService;
    private readonly ILanguageDetectionService languageDetectionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DetectedLanguageTranslatorMiddleware"/> class.
    /// </summary>
    /// <param name="translateToLanguage">The language to translate to.</param>
    /// <param name="languageExceptions">Collection of languages ​​that are the exception to translate.</param>
    /// <param name="languageDetectionServiceName">The name of a language detection cognitive service.</param>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="cognitiveServiceProvider">
    /// A cognitive service provider to locate a language detection and a text translation services from the given names.
    /// </param>
    public DetectedLanguageTranslatorMiddleware(CultureInfo translateToLanguage, IEnumerable<CultureInfo> languageExceptions, string languageDetectionServiceName, string textTranslationServiceName, ICognitiveServiceProvider cognitiveServiceProvider)
        : this(translateToLanguage, languageExceptions, cognitiveServiceProvider?.GetLanguageDetectionService(languageDetectionServiceName), cognitiveServiceProvider?.GetTextTranslationService(textTranslationServiceName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DetectedLanguageTranslatorMiddleware"/> class.
    /// </summary>
    /// <param name="translateToLanguage">The language to translate to.</param>
    /// <param name="languageExceptions">Collection of languages ​​that are the exception to translate.</param>
    /// <param name="languageDetectionService">A language detection cognitive service to detect the language to translate from.</param>
    /// <param name="textTranslationService">A text translation cognitive service to translate texts.</param>
    public DetectedLanguageTranslatorMiddleware(CultureInfo translateToLanguage, IEnumerable<CultureInfo> languageExceptions, ILanguageDetectionService languageDetectionService, ITextTranslationService textTranslationService)
    {
        Guard.IsNotNull(translateToLanguage);
        Guard.IsNotNull(languageDetectionService);
        Guard.IsNotNull(textTranslationService);

        this.translateToLanguage = translateToLanguage;
        this.languageExceptions = languageExceptions ?? Enumerable.Empty<CultureInfo>();
        translationService = textTranslationService;
        this.languageDetectionService = languageDetectionService;
    }

    /// <summary>
    /// <para>
    /// Processes an incoming activity and tries to detect its language. If the language is different than the translation language or
    /// any of the exception languages, then it will be translated into the translation language before passing it down the pipeline.
    /// </para>
    /// <para>
    /// When returning, it also process outgoint activities to translate back.
    /// </para>
    /// </summary>
    /// <param name="turnContext">The context object for this turn.</param>
    /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>A task that represents the work queued to execute.</returns>
    public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
    {
        if (turnContext?.Activity != null && turnContext.Activity.Type == ActivityTypes.Message && !string.IsNullOrWhiteSpace(turnContext.Activity.Text))
        {
            var message = turnContext.Activity.Text;

            var detectedLanguagesResult = await languageDetectionService.DetectLanguageAsync(new LanguageDetectionRequest()
            {
                Text = new[]
                {
                        new Text()
                        {
                            Id = DefaultRequestId,
                            Value = message,
                        },
                },
            }, cancellationToken);

            var detectedLanguage = detectedLanguagesResult.DetectedLanguages.OrderBy(l => l.ConfidenceScore).FirstOrDefault();

            if (detectedLanguage == null)
            {
                turnContext.Activity.Locale = detectedLanguage.Language.Name;

                if (!detectedLanguage.Language.Equals(translationService) && !languageExceptions.Contains(detectedLanguage.Language))
                {
                    await turnContext.TraceActivityAsync($@"{nameof(DetectedLanguageTranslatorMiddleware)} Trace", detectedLanguage, nameof(detectedLanguage), $@"{nameof(DetectedLanguageTranslatorMiddleware)} {nameof(OnTurnAsync)} Detected Language", cancellationToken);

                    var translationResults = await translationService.TranslateAsync(new TextTranslationRequest()
                    {
                        FromLanguage = detectedLanguage.Language,
                        Texts = new Dictionary<string, string>()
                    {
                        { DefaultRequestId, message },
                    },
                        ToLanguages = new[] { translateToLanguage },
                    }, cancellationToken);

                    var translationResult = translationResults.TextTranslations.OrderBy(t => t.ConfidenceScore).SingleOrDefault();

                    await turnContext.TraceActivityAsync($@"{nameof(DetectedLanguageTranslatorMiddleware)} Trace", translationResult, nameof(translationResult), $@"{nameof(DetectedLanguageTranslatorMiddleware)} {nameof(OnTurnAsync)} Text Translate - IN", cancellationToken);

                    turnContext.Activity.Text = TranslatorUtils.GetTranslation(translationResult?.Translations, translateToLanguage, message);

                    turnContext.OnSendActivities(async (_, activities, nextUpdate) =>
                    {
                        await TranslatorUtils.TranslateMessagesAsync(translationService, activities, translateToLanguage, detectedLanguage.Language, cancellationToken);

                        // Run full pipeline...
                        return await nextUpdate();
                    });
                }
            }
        }

        if (next != null)
        {
            await next(cancellationToken);
        }
    }
}

#pragma warning restore S103 // Lines should not be too long
