namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the specific type of operation from a <see cref="IMemoryStoreExtender"/>.
/// </summary>
public enum MemoryStoreEventTypes
{
    /// <summary>
    /// 'DoesCollectionExist' event type, raised when a collection is checked for existence.
    /// </summary>
    DoesCollectionExist = 0,

    /// <summary>
    /// 'CreateCollection' event type, raised when a collection is created.
    /// </summary>
    CreateCollection = 1,

    /// <summary>
    /// 'DeleteCollection' event type, raised when a collection is deleted.
    /// </summary>
    DeleteCollection = 2,

    /// <summary>
    /// 'Get' event type.
    /// </summary>
    [Obsolete("This event type is deprecated and will be removed in future versions. Please use 'GetMemory' instead.")]
    Get = GetMemory,

    /// <summary>
    /// 'GetMemory' event type, raised when a memory is retrieved.
    /// </summary>
    GetMemory = 3,

    /// <summary>
    /// 'UpsertMemory' event type, raised when a memory is updated or inserted.
    /// </summary>
    UpsertMemory = 4,

    /// <summary>
    /// 'DeleteMemory' event type, raised when a memory is deleted.
    /// </summary>
    DeleteMemory = 5,

    /// <summary>
    /// 'GetBatch' event type, raised when a batch of memories are retrieved.
    /// </summary>
    GetBatch = 6,

    /// <summary>
    /// 'UpsertBatch' event type, raised when a batch of memories are updated or inserted.
    /// </summary>
    UpsertBatch = 7,

    /// <summary>
    /// 'RemoveBatch' event type, raised when a batch of memories are deleted.
    /// </summary>
    RemoveBatch = 8,

    /// <summary>
    /// 'Exists' event type, raised when a memory is checked for existence.
    /// </summary>
    ExistsMemory = 9,
}
