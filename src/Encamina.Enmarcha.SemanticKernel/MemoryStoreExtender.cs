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
/// Extender that provides some CRUD operations over memories with multiple chunks that need to be extended by an <see cref="IMemoryStore"/>, using batch operations.
/// </summary>
public class MemoryStoreExtender : IMemoryStoreExtender
{
    private const string ChunkSize = @"chunkSize";

    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStoreExtender"/> class.
    /// </summary>
    /// <param name="memoryStore">A valid instance of a <see cref="IMemoryStore"/> to extend.</param>
    /// <param name="logger">Log service.</param>
    public MemoryStoreExtender(IMemoryStore memoryStore, ILogger<MemoryStoreExtender> logger)
    {
        MemoryStore = memoryStore;
        this.logger = logger;
    }

    /// <inheritdoc cref="IMemoryStoreExtender.MemoryStoreEvent" />
    public event EventHandler<MemoryStoreEventArgs> MemoryStoreEvent;

    /// <inheritdoc cref="IMemoryStoreExtender.MemoryStore" />
    public IMemoryStore MemoryStore { get; init; }

    /// <inheritdoc cref="IMemoryStoreExtender.OnMemoryStore(MemoryStoreEventArgs)" />
    public void OnMemoryStore(MemoryStoreEventArgs e) => throw new NotImplementedException();

    /// <inheritdoc cref="IMemoryStoreExtender.UpsertMemoryAsync(string, string, IEnumerable{string}, Kernel, IDictionary{string, string}, CancellationToken)" />
    public virtual async Task UpsertMemoryAsync(string memoryId, string collectionName, IEnumerable<string> chunks, Kernel kernel, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        await DeleteMemoryAsync(memoryId, collectionName, cancellationToken);
        await SaveChunks(memoryId, collectionName, chunks, metadata, kernel, cancellationToken);

        OnMemoryStore(new() { EventType = MemoryStoreEventTypes.Upsert, MemoryId = memoryId, CollectionName = collectionName });
    }

    /// <inheritdoc cref="IMemoryStoreExtender.DeleteMemoryAsync(string, string, CancellationToken)" />
    public virtual async Task DeleteMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);
        if (chunkSize > 0)
        {
            await MemoryStore.RemoveBatchAsync(collectionName, Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken);

            OnMemoryStore(new() { EventType = MemoryStoreEventTypes.Delete, MemoryId = memoryId, CollectionName = collectionName });
        }
    }

    /// <inheritdoc cref="IMemoryStoreExtender.GetMemoryAsync(string, string, CancellationToken)" />
    public virtual async Task<MemoryContent> GetMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (chunkSize == 0)
        {
            return null;
        }

        var memoryRecords = await MemoryStore.GetBatchAsync(collectionName, Enumerable.Range(0, chunkSize)
            .Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken: cancellationToken)
            .ToListAsync(cancellationToken);

        return new MemoryContent
        {
            Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(memoryRecords[0].Metadata.AdditionalMetadata),
            Chunks = memoryRecords.Select(m => m.Metadata.Text),
        };
    }

    /// <inheritdoc cref="IMemoryStoreExtender.BatchUpsertMemoriesAsync(string, IDictionary{string, MemoryContent}, Kernel, CancellationToken)" />
    public virtual async IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, Kernel kernel, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var memoryRecords = new Collection<MemoryRecord>();
        var memoryIds = new List<string>();

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

        foreach (var memoryId in memoryContents.Select(content => content.Key))
        {
            OnMemoryStore(new() { EventType = MemoryStoreEventTypes.Upsert, MemoryId = memoryId, CollectionName = collectionName });
        }

        await foreach (var item in memoryRecordsUniqueIdentifiers)
        {
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
