namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// <see cref="IMemoryManager.MemoryStorageEvent"/> arguments.
/// </summary>
public sealed class MemoryStorageEventArgs : EventArgs
{
    /// <summary>
    /// Gets event type.
    /// </summary>
    public MemoryStorageEventEnum EventType { get; init; }

    /// <summary>
    /// Gets the memory identifier.
    /// </summary>
    public string MemoryId { get; init; }

    /// <summary>
    /// Gets the collection name.
    /// </summary>
    public string CollectionName { get; init; }
}
