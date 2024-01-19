// Ignore Spelling: Utc

using System.Collections.Concurrent;

using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Base class for memory stores (i.e., <see cref="IMemoryStore"/>) handlers.
/// </summary>
public abstract class MemoryStoreHandlerBase : IMemoryStoreHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStoreHandlerBase"/> class.
    /// </summary>
    /// <param name="memoryManager">A valid instance of <see cref="IMemoryManager"/> that manages the memory store handled by this instance.</param>
    protected MemoryStoreHandlerBase(IMemoryManager memoryManager)
    {
        MemoryManager = memoryManager;
    }

    /// <inheritdoc/>
    public virtual IMemoryManager MemoryManager { get; }

    /// <inheritdoc/>
    public virtual string CollectionNamePostfix { get; init; }

    /// <inheritdoc/>
    public virtual string CollectionNamePrefix { get; init; }

    /// <summary>
    /// Gets the current collection of memory stores.
    /// </summary>
    protected IDictionary<string, MemoryStoreCollection> MemoryStoreCollectionInfo { get; } = new ConcurrentDictionary<string, MemoryStoreCollection>();

    /// <inheritdoc/>
    public virtual async Task<string> GetCollectionNameAsync(string collectionId, CancellationToken cancellationToken)
    {
        if (MemoryStoreCollectionInfo.TryGetValue(collectionId, out var memoryStoreInfo))
        {
            memoryStoreInfo.LastAccessUtc = DateTime.UtcNow;
            return memoryStoreInfo.CollectionName;
        }

        var collectionName = $@"{CollectionNamePrefix}{collectionId}{CollectionNamePostfix}";

        MemoryStoreCollectionInfo[collectionId] = new MemoryStoreCollection()
        {
            CollectionName = collectionName,
            LastAccessUtc = DateTime.UtcNow,
        };

        if (!await MemoryManager.MemoryStore.DoesCollectionExistAsync(collectionName, cancellationToken))
        {
            await MemoryManager.MemoryStore.CreateCollectionAsync(collectionName, cancellationToken);
        }

        return MemoryStoreCollectionInfo[collectionId].CollectionName;
    }

    /// <summary>
    /// Provides information about a collection in a memory store.
    /// </summary>
    protected sealed class MemoryStoreCollection
    {
        /// <summary>
        /// Gets the collection name.
        /// </summary>
        public string CollectionName { get; init; }

        /// <summary>
        /// Gets or sets the last access date in UTC.
        /// </summary>
        public DateTime LastAccessUtc { get; set; }
    }
}
