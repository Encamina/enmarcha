// Ignore Spelling: Upsert

using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Azure;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// This class provides some additional CRUD operations over memories with multiple chunks that extends operations from a <see cref="IMemoryStore"/>.
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

    /// <inheritdoc/>
    public event EventHandler<MemoryStoreEventArgs> MemoryStoreEvent;

    /// <inheritdoc/>
    public IMemoryStore MemoryStore { get; init; }

    /// <inheritdoc/>
    public void RaiseMemoryStoreEvent(MemoryStoreEventArgs e) => MemoryStoreEvent?.Invoke(this, e);

    /// <inheritdoc/>
    public virtual async Task UpsertMemoryAsync(string memoryId, string collectionName, IEnumerable<string> chunks, Kernel kernel, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
    {
        await DeleteMemoryAsync(memoryId, collectionName, cancellationToken);
        await SaveChunks(memoryId, collectionName, chunks, metadata, kernel, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);
        if (chunkSize > 0)
        {
            var keys = Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i));
            await MemoryStore.RemoveBatchAsync(collectionName, keys, cancellationToken);
            RaiseMemoryStoreEvent(new() { EventType = MemoryStoreEventTypes.RemoveBatch, Keys = keys, CollectionName = collectionName });
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

        var keys = Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i));
        var memoryRecords = await MemoryStore.GetBatchAsync(collectionName, keys, cancellationToken: cancellationToken).ToListAsync(cancellationToken);
        RaiseMemoryStoreEvent(new() { EventType = MemoryStoreEventTypes.GetBatch, Keys = keys, CollectionName = collectionName });

        return new MemoryContent
        {
            Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(memoryRecords[0].Metadata.AdditionalMetadata),
            Chunks = memoryRecords.Select(m => m.Metadata.Text),
        };
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, Kernel kernel, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var memoryRecords = new List<MemoryRecord>();
        var textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        foreach (var memoryContentItem in memoryContents)
        {
            var memoryContentId = memoryContentItem.Key;
            var memoryContent = memoryContentItem.Value;
            var totalChunks = memoryContent.Chunks.Count();

            if (totalChunks > 0)
            {
                memoryContent.Metadata.Add(Constants.MetadataTotalChunksCount, totalChunks.ToString());

                var embeddingTasks = memoryContent.Chunks.Select(async (chunk, i) =>
                {
                    var embedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(chunk, kernel, cancellationToken);
                    return MemoryRecord.LocalRecord($@"{memoryContentId}-{i}", chunk, null, embedding, JsonSerializer.Serialize(memoryContent.Metadata));
                });

                var records = await Task.WhenAll(embeddingTasks);
                memoryRecords.AddRange(records);
            }
        }

        var asyncKeys = MemoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken);

        List<string> keys = [];

        await foreach (var key in asyncKeys)
        {
            logger.LogInformation(@"Processed memory record {item}.", key);

            keys.Add(key);

            yield return key;
        }

        RaiseMemoryStoreEvent(new() { EventType = MemoryStoreEventTypes.UpsertBatch, Keys = keys, CollectionName = collectionName });
    }

    private static string BuildMemoryIdentifier(string memoryId, int chunkIndex) => $@"{memoryId}-{chunkIndex}";

    private async Task<int> GetChunkSize(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var key = BuildMemoryIdentifier(memoryId, 0);
        MemoryRecord firstMemoryChunk = null;

        try
        {
            firstMemoryChunk = await MemoryStore.GetAsync(collectionName, key, cancellationToken: cancellationToken);
        }
        catch (RequestFailedException e) when (e.Status == (int)HttpStatusCode.NotFound)
        {
            // At this point, we need to catch the NotFound exception. This is necessary because, in the case of Azure AI Search Memory Store,
            // if the element does not exist, it throws an exception instead of returning a null value, which would be the expected behavior.
            // We have opened an issue in Semantic Kernel and also an issue in ENMARCHA to track the progress of this issue and remove this try/catch block once it is resolved.
            // Issue ENMARCHA: https://github.com/Encamina/enmarcha/issues/72

            /* Do nothing */
        }

        RaiseMemoryStoreEvent(new() { EventType = MemoryStoreEventTypes.Get, Keys = [key], CollectionName = collectionName });

        if (firstMemoryChunk == null)
        {
            return 0;
        }

        var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(firstMemoryChunk.Metadata.AdditionalMetadata);

        return int.Parse(metadata[ChunkSize]);
    }

    private async Task SaveChunks(string memoryId, string collectionName, IEnumerable<string> chunks, IDictionary<string, string> metadata, Kernel kernel, CancellationToken cancellationToken)
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
            memoryRecords.Add(MemoryRecord.LocalRecord(BuildMemoryIdentifier(memoryId, i), chunk, null, embedding, metadataJson));
        }

        await MemoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken).ToListAsync(cancellationToken: cancellationToken);
        var keys = memoryRecords.Select(r => r.Key);
        RaiseMemoryStoreEvent(new() { EventType = MemoryStoreEventTypes.UpsertBatch, Keys = keys, CollectionName = collectionName });
    }
}
