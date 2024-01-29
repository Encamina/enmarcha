namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the specific type of operation from a <see cref="IMemoryStoreExtender"/>.
/// </summary>
public enum MemoryStoreEventTypes
{
    /// <summary>
    /// 'CreateCollection' event type.
    /// </summary>
    CreateCollection = 0,

    /// <summary>
    /// 'DeleteCollection' event type.
    /// </summary>
    DeleteCollection = 1,

    /// <summary>
    /// 'Upsert' event type.
    /// </summary>
    Upsert = 2,

    /// <summary>
    /// 'Delete' event type.
    /// </summary>
    Delete = 3,
}
