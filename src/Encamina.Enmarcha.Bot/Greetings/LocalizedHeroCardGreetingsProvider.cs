using System.Globalization;

using Encamina.Enmarcha.Bot.Abstractions.Extensions;
using Encamina.Enmarcha.Bot.Abstractions.Greetings;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TraceExtensions;

using Microsoft.Bot.Schema;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Greetings;

/// <summary>
/// Sends a greetings message using a <see cref="HeroCard">hero card</see>.
/// </summary>
internal class LocalizedHeroCardGreetingsProvider : GreetingsProviderBase
{
    private readonly ILogger<LocalizedHeroCardGreetingsProvider> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedHeroCardGreetingsProvider"/> class.
    /// </summary>
    /// <param name="options">Greetings options with values to build a <see cref="HeroCard">hero card</see>.</param>
    /// <param name="logger">A logger for this greetings provider.</param>
    public LocalizedHeroCardGreetingsProvider(ILocalizedHeroCardGreetingsOptions options, ILogger<LocalizedHeroCardGreetingsProvider> logger)
    {
        this.logger = logger;
        Options = options;
    }

    /// <summary>
    /// Gets the current greetings options with values to build a <see cref="HeroCard">hero card</see>.
    /// </summary>
    protected ILocalizedHeroCardGreetingsOptions Options { get; }

    /// <inheritdoc/>
    public async override Task SendAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        var activityLocal = turnContext.Activity.GetCultureInfoFromActivity();

        await turnContext.TraceActivityAsync($@"{nameof(LocalizedHeroCardGreetingsProvider)} Trace", activityLocal, typeof(CultureInfo).ToString(), nameof(turnContext.Activity.Locale), cancellationToken);

        CultureInfo key;

        if (Options.LocalizedOptions.ContainsKey(activityLocal))
        {
            key = activityLocal;
        }
        else if (Options.LocalizedOptions.ContainsKey(activityLocal.Parent))
        {
            key = activityLocal.Parent;
        }
        else
        {
            key = Options.DefaultLocale;
        }

        if (Options.LocalizedOptions.TryGetValue(key, out var options))
        {
            turnContext.Activity.Locale = key.Name;  // Set activity locale to the used for the greetings, specially if using the dafault locale.

            foreach (var option in options)
            {
                var cardAttachment = new HeroCard()
                {
                    Images = option.Images,
                    Subtitle = option.Subtitle,
                    Text = option.Text,
                    Title = option.Title,
                }.ToAttachment();

                await turnContext.TraceActivityAsync($@"{nameof(LocalizedHeroCardGreetingsProvider)} Trace", cardAttachment, typeof(Attachment).ToString(), nameof(SendAsync), cancellationToken);
                var response = await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
                logger.LogInformation($@"Greetings message sent with response ID {response.Id}");
            }
        }
        else
        {
            var message = $@"{nameof(LocalizedHeroCardGreetingsProvider)} {nameof(SendAsync)} - Missing greetings options for default locale '{Options.DefaultLocale}'!";

            logger.LogWarning(message);
            await turnContext.TraceActivityAsync($@"{nameof(LocalizedHeroCardGreetingsProvider)} Trace", message, null, nameof(SendAsync), cancellationToken);
        }
    }
}
