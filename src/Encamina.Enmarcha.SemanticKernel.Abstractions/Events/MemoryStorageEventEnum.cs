using System.ComponentModel;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// <see cref="IMemoryManager.MemoryStorageEvent"/> types.
/// </summary>
public enum MemoryStorageEventEnum
{
    /// <summary>
    /// 'Undefined' event type.
    /// </summary>
    [Description("Undefined")]
    Undefined = 0,

    /// <summary>
    /// 'Get' event type.
    /// </summary>
    [Description("Get")]
    Get = 1,

    /// <summary>
    /// 'Upsert' event type.
    /// </summary>
    [Description("Upsert")]
    Upsert = 2,

    /// <summary>
    /// 'UpsertBatch' event type.
    /// </summary>
    [Description("UpsertBatch")]
    UpsertBatch = 3,

    /// <summary>
    /// 'Delete' event type.
    /// </summary>
    [Description("Delete")]
    Delete = 4,
}
