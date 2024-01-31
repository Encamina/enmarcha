namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the specific type of operation from a <see cref="IMemoryStoreExtender"/>.
/// </summary>
public enum MemoryStoreEventTypes
{
    /// <summary>
    /// 'DoesCollectionExist' event type.
    /// </summary>
    DoesCollectionExist = 0,

    /// <summary>
    /// 'CreateCollection' event type.
    /// </summary>
    CreateCollection = 1,

    /// <summary>
    /// 'DeleteCollection' event type.
    /// </summary>
    DeleteCollection = 2,

    /// <summary>
    /// 'Get' event type.
    /// </summary>
    Get = 3,

    /// <summary>
    /// 'UpsertMemory' event type.
    /// </summary>
    UpsertMemory = 4,

    /// <summary>
    /// 'DeleteMemory' event type.
    /// </summary>
    DeleteMemory = 5,

    /// <summary>
    /// 'GetBatch' event type.
    /// </summary>
    GetBatch = 6,

    /// <summary>
    /// 'UpsertBatch' event type.
    /// </summary>
    UpsertBatch = 7,

    /// <summary>
    /// 'RemoveBatch' event type.
    /// </summary>
    RemoveBatch = 8,
}
