using System.Collections.Concurrent;
using System.Runtime.Serialization;

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
    /// <param name="memoryStore">The <see cref="IMemoryStore"/> to handle.</param>
#pragma warning disable SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    protected MemoryStoreHandlerBase(IMemoryStore memoryStore)
#pragma warning restore SKEXP0003 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    {
        MemoryStore = memoryStore;
    }

    /// <inheritdoc/>
    public abstract string CollectionNamePostfix { get; init; }

    /// <inheritdoc/>
    public abstract string CollectionNamePrefix { get; init; }

    /// <summary>
    /// Gets the current collection of memory stores.
    /// </summary>
    protected IDictionary<string, MemoryStoreCollection> MemoryStoreCollectionInfo { get; } = new ConcurrentDictionary<string, MemoryStoreCollection>();

    /// <summary>
    /// Gets the current <see cref="IMemoryStore"/> handled by this instance.
    /// </summary>
    protected IMemoryStore MemoryStore { get; }

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

        if (!await MemoryStore.DoesCollectionExistAsync(collectionName, cancellationToken))
        {
            await MemoryStore.CreateCollectionAsync(collectionName, cancellationToken);
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
        [Obsolete("Use 'LastAccessUTC' property instead.")]
        public DateTime LastAccessUtc { get; set; }

        /// <summary>
        /// Gets or sets the last access date in UTC.
        /// </summary>
        public DateTime LastAccessUTC
        {
            get => LastAccessUtc;
            set => LastAccessUtc = value;
        }
    }
}
