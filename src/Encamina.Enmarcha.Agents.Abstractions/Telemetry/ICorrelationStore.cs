namespace Encamina.Enmarcha.Agents.Abstractions.Telemetry;

/// <summary>
/// Defines methods for storing and retrieving correlation information for conversations and activities.
/// </summary>
public interface ICorrelationStore
{
    /// <summary>
    /// Stores a correlation entry for a given conversation and activity with a specified time-to-live (TTL).
    /// </summary>
    /// <param name="conversationId">The unique identifier for the conversation.</param>
    /// <param name="activityId">The unique identifier for the activity.</param>
    /// <param name="entry">The correlation entry to store.</param>
    /// <param name="ttl">The time-to-live for the entry.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask SetAsync(string conversationId, string activityId, CorrelationEntry entry, TimeSpan ttl, CancellationToken ct);

    /// <summary>
    /// Retrieves a correlation entry for a given conversation and activity.
    /// </summary>
    /// <param name="conversationId">The unique identifier for the conversation.</param>
    /// <param name="activityId">The unique identifier for the activity.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The correlation entry if found; otherwise, <c>null</c>.</returns>
    ValueTask<CorrelationEntry?> GetAsync(string conversationId, string activityId, CancellationToken ct);
}
