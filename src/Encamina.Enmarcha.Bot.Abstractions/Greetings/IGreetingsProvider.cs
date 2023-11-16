using Microsoft.Bot.Builder;

namespace Encamina.Enmarcha.Bot.Abstractions.Greetings;

/// <summary>
/// Represents a provider for greetings messages.
/// </summary>
public interface IGreetingsProvider
{
    /// <summary>
    /// Sends a greetings message.
    /// </summary>
    /// <param name="turnContext">The current turn context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task SendAsync(ITurnContext turnContext, CancellationToken cancellationToken);

    /// <summary>
    /// A greetings message might be a template with tokens to be replaced before sending it. This method should be called
    /// before <see cref="SendAsync"/> to add any template properties that should be consider as tokens for the greetings
    /// message.
    /// </summary>
    /// <param name="templateProperties">
    /// A dictionalry with values to use as replacements for tokens or parameters in the greetings message.
    /// </param>
    void AddGreetingsTemplateProperties(IDictionary<string, object> templateProperties);
}
