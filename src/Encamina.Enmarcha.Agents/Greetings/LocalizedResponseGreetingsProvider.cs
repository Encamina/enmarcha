using System.Globalization;

using Encamina.Enmarcha.Agents.Abstractions.Greetings;
using Encamina.Enmarcha.Conversation.Abstractions;
using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Greetings;

/// <summary>
/// A custom implementation of a <see cref="IGreetingsProvider">greetings provider</see> based on
/// responses retrieved from an <see cref="IIntentResponsesProvider"/>.
/// </summary>
internal class LocalizedResponseGreetingsProvider : GreetingsProviderBase
{
    private readonly string defaultLocale;
    private readonly string intentName;
    private readonly IIntentResponsesProvider responsesProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizedResponseGreetingsProvider"/> class.
    /// </summary>
    /// <param name="responsesProvider">A valid instance of an <see cref="IIntentResponsesProvider"/>.</param>
    /// <param name="defaultLocale">Default locale.</param>
    /// <param name="intentName">The expected greetings intent name. Default value is '<c>Greetings</c>'.</param>
    public LocalizedResponseGreetingsProvider(IIntentResponsesProvider responsesProvider, string defaultLocale, string intentName = @"Greetings")
    {
        this.responsesProvider = responsesProvider;
        this.defaultLocale = defaultLocale;
        this.intentName = intentName;
    }

    /// <inheritdoc/>
    public override async Task SendAsync(ITurnContext turnContext, CancellationToken cancellationToken)
    {
        if (turnContext != null)
        {
            var activityLocal = string.IsNullOrWhiteSpace(turnContext.Activity?.Locale) ? defaultLocale : turnContext.Activity.Locale;

            await turnContext.TraceActivityAsync($@"{nameof(LocalizedHeroCardGreetingsProvider)} Trace", activityLocal, typeof(CultureInfo).ToString(), @"Locale", cancellationToken);

            var responses = await responsesProvider.GetResponsesAsync(intentName, activityLocal, cancellationToken);

            foreach (var response in responses)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response.Text.TemplateStringFormatterWithValues(GreetingsTemplateProperties)), cancellationToken);
            }
        }
    }
}
