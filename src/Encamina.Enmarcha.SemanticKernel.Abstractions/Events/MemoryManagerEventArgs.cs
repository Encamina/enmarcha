namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the event arguments of an <see cref="IMemoryManager.MemoryManagerEvent"/>.
/// </summary>
public sealed class MemoryManagerEventArgs : EventArgs
{
    /// <summary>
    /// Gets event type.
    /// </summary>
    public MemoryManagerEventTypes EventType { get; init; }

    /// <summary>
    /// Gets the memory identifier.
    /// </summary>
    public string MemoryId { get; init; }

    /// <summary>
    /// Gets the collection name.
    /// </summary>
    public string CollectionName { get; init; }
}
