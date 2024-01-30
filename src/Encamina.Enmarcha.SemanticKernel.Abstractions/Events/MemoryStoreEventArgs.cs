namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the event arguments of an <see cref="IMemoryStoreExtender.MemoryStoreEvent"/>.
/// </summary>
public sealed class MemoryStoreEventArgs : EventArgs
{
    /// <summary>
    /// Gets event type.
    /// </summary>
    public MemoryStoreEventTypes EventType { get; init; }

    /// <summary>
    /// Gets the memory identifiers.
    /// </summary>
    public IEnumerable<string> Keys { get; init; } = [];

    /// <summary>
    /// Gets the collection name.
    /// </summary>
    public string CollectionName { get; init; } = string.Empty;
}
