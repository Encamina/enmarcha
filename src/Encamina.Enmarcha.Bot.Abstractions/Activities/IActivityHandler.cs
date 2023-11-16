using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.Activities;

/// <summary>
/// Represents a handler for a <see cref="ITurnContext">turn context</see>.
/// </summary>
public interface IActivityHandler : IOrderable
{
    /// <summary>
    /// Gets the process time for this handler.
    /// </summary>
    HandlerProcessTimes ProcessTime { get; }

    /// <summary>
    /// Handles an activity.
    /// </summary>
    /// <param name="activity">The activity to handle.</param>
    /// <param name="turnContext">The current turn context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns <see langword="true"/> if an answer is successfully handled and used to send a response, otherwise returns <see langword="false"/>.
    /// </returns>
    Task<bool> HandleAsync(IActivity activity, ITurnContext turnContext, CancellationToken cancellationToken);
}
