// Ignore Spelling: Upsert

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// Manager that provides some CRUD operations over memories with multiple chunks that need to be managed by an <see cref="IMemoryStore"/>, using batch operations.
/// </summary>
public class MemoryManager : IMemoryManager
{
    private const string ChunkSize = @"chunkSize";

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryManager"/> class.
    /// </summary>
    /// <param name="memoryStore">A valid instance of a <see cref="IMemoryStore"/> to manage.</param>
    /// <param name="memoryStorageEventHandler">A delegate to handle events from <see cref="MemoryManagerEvent"/>.</param>
    /// <param name="logger">Log service.</param>
    public MemoryManager(IMemoryStore memoryStore, ILogger<MemoryManager> logger, Action<object, MemoryManagerEventArgs> memoryStorageEventHandler)
    {
        MemoryStore = memoryStore;
        this.logger = logger;

        if (memoryStorageEventHandler is not null)
        {
            MemoryManagerEvent += (sender, args) => memoryStorageEventHandler?.Invoke(sender ?? nameof(MemoryManagerEventTypes.Undefined), args);
        }
    }

    /// <inheritdoc/>
    public event EventHandler<MemoryManagerEventArgs> MemoryManagerEvent;

    /// <inheritdoc/>
    public IMemoryStore MemoryStore { get; init; }

    /// <inheritdoc/>
    public virtual async Task UpsertMemoryAsync(string memoryId, string collectionName, IEnumerable<string> chunks, Kernel kernel, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        var memoryChunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (memoryChunkSize > 0)
        {
            await DeleteMemoryAsync(memoryId, collectionName, memoryChunkSize, cancellationToken);
        }

        await SaveChunks(memoryId, collectionName, chunks, metadata, kernel, cancellationToken);

        MemoryManagerEvent?.Invoke(this, new() { EventType = MemoryManagerEventTypes.Upsert, MemoryId = memoryId, CollectionName = collectionName });
    }

    /// <inheritdoc/>
    public virtual async Task DeleteMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (chunkSize > 0)
        {
            await DeleteMemoryAsync(memoryId, collectionName, chunkSize, cancellationToken);

            MemoryManagerEvent?.Invoke(this, new() { EventType = MemoryManagerEventTypes.Delete, MemoryId = memoryId, CollectionName = collectionName });
        }
    }

    /// <inheritdoc/>
    public virtual async Task<MemoryContent> GetMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (chunkSize == 0)
        {
            return null;
        }

        var memoryRecords = await MemoryStore.GetBatchAsync(collectionName, Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken: cancellationToken)
                                             .ToListAsync(cancellationToken);

        MemoryManagerEvent?.Invoke(this, new() { EventType = MemoryManagerEventTypes.Get, MemoryId = memoryId, CollectionName = collectionName });

        return new MemoryContent
        {
            Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(memoryRecords[0].Metadata.AdditionalMetadata),
            Chunks = memoryRecords.Select(m => m.Metadata.Text),
        };
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, Kernel kernel, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var memoryRecords = new Collection<MemoryRecord>();

        foreach (var memoryContentItem in memoryContents)
        {
            var memoryContentId = memoryContentItem.Key;
            var memoryContent = memoryContentItem.Value;
            var totalChunks = memoryContent.Chunks.Count();

            if (totalChunks > 0)
            {
                memoryContent.Metadata.Add(Constants.MetadataTotalChunksCount, totalChunks.ToString());

                for (var i = 0; i < totalChunks; i++)
                {
                    var chunk = memoryContent.Chunks.ElementAt(i);
                    var embedding = await kernel.GetRequiredService<ITextEmbeddingGenerationService>().GenerateEmbeddingAsync(chunk, kernel, cancellationToken);
                    memoryRecords.Add(MemoryRecord.LocalRecord($@"{memoryContentId}-{i}", chunk, null, embedding, JsonSerializer.Serialize(memoryContent.Metadata)));
                }
            }
        }

        var memoryRecordsUniqueIdentifiers = MemoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken);

        await foreach (var item in memoryRecordsUniqueIdentifiers)
        {
            MemoryManagerEvent?.Invoke(this, new() { EventType = MemoryManagerEventTypes.UpsertBatch, MemoryId = item, CollectionName = collectionName });

            logger.LogInformation(@"Processed memory record {item}.", item);

            yield return item;
        }
    }

    private static string BuildMemoryIdentifier(string memoryId, int chunkIndex) => $@"{memoryId}-{chunkIndex}";

    private async Task<int> GetChunkSize(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var fistMemoryChunk = await MemoryStore.GetAsync(collectionName, BuildMemoryIdentifier(memoryId, 0), cancellationToken: cancellationToken);

        if (fistMemoryChunk == null)
        {
            return 0;
        }

        var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(fistMemoryChunk.Metadata.AdditionalMetadata);

        return int.Parse(metadata[ChunkSize]);
    }

    private async Task DeleteMemoryAsync(string memoryId, string collectionName, int chunkSize, CancellationToken cancellationToken)
    {
        await MemoryStore.RemoveBatchAsync(collectionName, Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken);
    }

    private async Task SaveChunks(string memoryid, string collectionName, IEnumerable<string> chunks, IDictionary<string, string> metadata, Kernel kernel, CancellationToken cancellationToken)
    {
        metadata ??= new Dictionary<string, string>();

        var chunksCount = chunks.Count();

        metadata[ChunkSize] = chunksCount.ToString();

        var metadataJson = JsonSerializer.Serialize(metadata);

        var memoryRecords = new Collection<MemoryRecord>();

        for (var i = 0; i < chunksCount; i++)
        {
            var chunk = chunks.ElementAt(i);
            var embedding = await kernel.GetRequiredService<ITextEmbeddingGenerationService>().GenerateEmbeddingAsync(chunk, kernel, cancellationToken);
            memoryRecords.Add(MemoryRecord.LocalRecord(BuildMemoryIdentifier(memoryid, i), chunk, null, embedding, metadataJson));
        }

        await MemoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken).ToListAsync(cancellationToken: cancellationToken);
    }
}
