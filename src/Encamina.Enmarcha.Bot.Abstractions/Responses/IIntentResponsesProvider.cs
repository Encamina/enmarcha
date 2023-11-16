using System.Globalization;

namespace Encamina.Enmarcha.Bot.Abstractions.Responses;

/// <summary>
/// Represents a bot's response provider from intents.
/// </summary>
public interface IIntentResponsesProvider
{
    /// <summary>
    /// Retrieves responses from the given <paramref name="intent"/> in the specified <paramref name="locale"/>.
    /// </summary>
    /// <param name="intent">The intent of the responses to get.</param>
    /// <param name="locale">The locale for the responses to get.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A read only collection of responses that satisfies the given <paramref name="intent"/> in the also given <paramref name="locale"/>.
    /// </returns>
    Task<IReadOnlyCollection<Response>> GetResponsesAsync(string intent, string locale, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves responses from the given <paramref name="intent"/> in the specified <paramref name="culture"/>.
    /// </summary>
    /// <param name="intent">The intent of the responses to get.</param>
    /// <param name="culture">The culture for the responses to get.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A read only collection of responses that satisfies the given <paramref name="intent"/> in the also given <paramref name="culture"/>.
    /// </returns>
    Task<IReadOnlyCollection<Response>> GetResponsesAsync(string intent, CultureInfo culture, CancellationToken cancellationToken);
}
