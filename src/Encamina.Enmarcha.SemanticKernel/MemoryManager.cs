// Ignore Spelling: Upsert

using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// Manager that provides some CRUD operations over memories with multiple chunks that need to be managed by an <see cref="IMemoryStore"/>, using batch operations.
/// </summary>
[Obsolete("This class is obsolete and will be removed in a future version. Use the Encamina.Enmarcha.SemanticKernel.MemoryStoreExtender class instead.", false)]
public class MemoryManager : MemoryStoreExtender, IMemoryManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryManager"/> class.
    /// </summary>
    /// <param name="memoryStore">A valid instance of a <see cref="IMemoryStore"/> to manage.</param>
    /// <param name="logger">Log service.</param>
    public MemoryManager(IMemoryStore memoryStore, ILogger<MemoryManager> logger) : base(memoryStore, logger)
    {
    }
}
