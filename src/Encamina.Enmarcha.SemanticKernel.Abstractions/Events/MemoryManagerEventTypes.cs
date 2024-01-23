namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// Represents the specific type of operation from a <see cref="IMemoryManager"/>.
/// </summary>
public enum MemoryManagerEventTypes
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
