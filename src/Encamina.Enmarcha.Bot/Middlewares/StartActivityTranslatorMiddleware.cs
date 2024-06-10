using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Encamina.Enmarcha.Bot.Abstractions.Extensions;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Middlewares;

/// <summary>
/// Middleware to automatically translate messages sent to consumers during start activities into the language
/// received as <see cref="Activity.Locale"/>.
/// </summary>
/// <remarks>
/// This middleware does not automatically detect the language because there is no input from the consumer yet.
/// </remarks>
public class StartActivityTranslatorMiddleware : IMiddleware
{
    private readonly ITextTranslationService translationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartActivityTranslatorMiddleware"/> class.
    /// </summary>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="cognitiveServiceProvider">
    /// A cognitive service provider to locate a language detection and a text translation services from the given names.
    /// </param>
    public StartActivityTranslatorMiddleware(string textTranslationServiceName, ICognitiveServiceProvider cognitiveServiceProvider)
        : this(cognitiveServiceProvider.GetTextTranslationService(textTranslationServiceName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StartActivityTranslatorMiddleware"/> class.
    /// </summary>
    /// <param name="textTranslationService">A text translation cognitive service to translate texts.</param>
    public StartActivityTranslatorMiddleware(ITextTranslationService textTranslationService)
    {
        Guard.IsNotNull(textTranslationService);

        translationService = textTranslationService;
    }

    /// <summary>
    /// Processes any start activity and translates any outgoing message to the <see cref="Activity.Locale"/> language.
    /// </summary>
    /// <param name="turnContext">The context object for this turn.</param>
    /// <param name="next">The delegate to call to continue the bot middleware pipeline.</param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>A task that represents the work queued to execute.</returns>
    public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
    {
        if (turnContext?.Activity?.IsStartActivity() == true)
        {
            var currentLanguage = turnContext.Activity.GetCultureInfoFromActivity();

            turnContext.OnSendActivities(async (_, activities, nextUpdate) =>
            {
                var messageActivities = activities.Where(a => a.Type == ActivityTypes.Message && !currentLanguage.MatchesWith(a.GetCultureInfoFromActivity())).ToList();

                if (messageActivities.Any())
                {
                    await TranslatorUtils.TranslateMessagesAsync(translationService, messageActivities, null, currentLanguage, cancellationToken);
                }

                // Run full pipeline...
                return await nextUpdate();
            });
        }

        if (next != null)
        {
            await next(cancellationToken);
        }
    }
}
