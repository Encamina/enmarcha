using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.Activities;

/// <summary>
/// Represents a processor for <see cref="IActivity">activities</see> using <see cref="IActivityHandler">handlers</see>.
/// </summary>
public interface IActivityProcessor
{
    /// <summary>
    /// Gets the current collection of available handlers.
    /// </summary>
    IEnumerable<IActivityHandler> Handlers { get; init; }

    /// <summary>
    /// Process activities at the beginning from the given <paramref name="turnContext">turn context</paramref>.
    /// </summary>
    /// <param name="activity">The activity to process.</param>
    /// <param name="turnContext">The current context for this turn of the bot.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the process has succeeded and no further processing is required, otherwise returns <see langword="false"/>.
    /// </returns>
    Task<bool> BeginProcessAsync(IActivity activity, ITurnContext turnContext, CancellationToken cancellationToken);

    /// <summary>
    /// Process activities at the end from the given <paramref name="turnContext">turn context</paramref>.
    /// </summary>
    /// <param name="activity">The activity to process.</param>
    /// <param name="turnContext">The current context for this turn of the bot.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the process has succeeded and no further processing is required, otherwise returns <see langword="false"/>.
    /// </returns>
    Task<bool> EndProcessAsync(IActivity activity, ITurnContext turnContext, CancellationToken cancellationToken);
}
