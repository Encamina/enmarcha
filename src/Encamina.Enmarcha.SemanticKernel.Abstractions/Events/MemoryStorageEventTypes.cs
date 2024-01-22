namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// <see cref="IMemoryManager.MemoryStorageEvent"/> types.
/// </summary>
public enum MemoryStorageEventTypes
{
    /// <summary>
    /// 'Undefined' event type.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// 'Get' event type.
    /// </summary>
    Get = 1,

    /// <summary>
    /// 'Upsert' event type.
    /// </summary>
    Upsert = 2,

    /// <summary>
    /// 'UpsertBatch' event type.
    /// </summary>
    UpsertBatch = 3,

    /// <summary>
    /// 'Delete' event type.
    /// </summary>
    Delete = 4,
}
