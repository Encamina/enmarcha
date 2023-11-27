// Ignore Spelling: Upsert

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.Embeddings;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// Manager that provides some CRUD operations over memories with multiple chunks that need to be managed by an <see cref="IMemoryStore"/>, using batch operations.
/// </summary>
public class MemoryManager : IMemoryManager
{
    private const string ChunkSize = @"chunkSize";

    private readonly ILogger logger;
    private readonly IMemoryStore memoryStore;
    private readonly ITextEmbeddingGeneration textEmbeddingGeneration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryManager"/> class.
    /// </summary>
    /// <param name="kernel">
    /// A valid instance of <see cref="IKernel"/>, used to get the configured text embeddings generation service (<see cref="ITextEmbeddingGeneration"/>) required by this manager.
    /// </param>
    /// <param name="memoryStore">A valid instance of a <see cref="IMemoryStore"/> to manage.</param>
    public MemoryManager(IKernel kernel, IMemoryStore memoryStore)
    {
        textEmbeddingGeneration = kernel.GetService<ITextEmbeddingGeneration>(); // If the service is not configured, a `KernelException` is thrown...

        this.logger = kernel.LoggerFactory.CreateLogger<MemoryManager>();
        this.memoryStore = memoryStore;
    }

    /// <inheritdoc/>
    public virtual async Task UpsertMemoryAsync(string memoryId, string collectionName, IEnumerable<string> chunks, CancellationToken cancellationToken, IDictionary<string, string> metadata = null)
    {
        var memoryChunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (memoryChunkSize > 0)
        {
            await DeleteMemoryAsync(memoryId, collectionName, memoryChunkSize, cancellationToken);
        }

        await SaveChunks(memoryId, collectionName, chunks, metadata, cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task DeleteMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var chunkSize = await GetChunkSize(memoryId, collectionName, cancellationToken);

        if (chunkSize > 0)
        {
            await DeleteMemoryAsync(memoryId, collectionName, chunkSize, cancellationToken);
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

        var memoryRecords = await memoryStore.GetBatchAsync(collectionName, Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken: cancellationToken)
                                             .ToListAsync(cancellationToken);

        return new MemoryContent
        {
            Metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(memoryRecords[0].Metadata.AdditionalMetadata),
            Chunks = memoryRecords.Select(m => m.Metadata.Text),
        };
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                    var embedding = await textEmbeddingGeneration.GenerateEmbeddingAsync(chunk, cancellationToken);
                    memoryRecords.Add(MemoryRecord.LocalRecord($@"{memoryContentId}-{i}", chunk, null, embedding, JsonSerializer.Serialize(memoryContent.Metadata)));
                }
            }
        }

        var memoryRecordsUniqueIdentifiers = memoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken);

        await foreach (var item in memoryRecordsUniqueIdentifiers)
        {
            logger.LogInformation($@"Processed memory record {item}.");
            yield return item;
        }
    }

    private static string BuildMemoryIdentifier(string memoryId, int chunkIndex) => $@"{memoryId}-{chunkIndex}";

    private async Task<int> GetChunkSize(string memoryId, string collectionName, CancellationToken cancellationToken)
    {
        var fistMemoryChunk = await memoryStore.GetAsync(collectionName, BuildMemoryIdentifier(memoryId, 0), cancellationToken: cancellationToken);

        if (fistMemoryChunk == null)
        {
            return 0;
        }

        var metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(fistMemoryChunk.Metadata.AdditionalMetadata);

        return int.Parse(metadata[ChunkSize]);
    }

    private async Task DeleteMemoryAsync(string memoryId, string collectionName, int chunkSize, CancellationToken cancellationToken)
    {
        await memoryStore.RemoveBatchAsync(collectionName, Enumerable.Range(0, chunkSize).Select(i => BuildMemoryIdentifier(memoryId, i)), cancellationToken);
    }

    private async Task SaveChunks(string memoryid, string collectionName, IEnumerable<string> chunks, IDictionary<string, string> metadata, CancellationToken cancellationToken)
    {
        metadata ??= new Dictionary<string, string>();

        var chunksCount = chunks.Count();

        metadata[ChunkSize] = chunksCount.ToString();

        var metadataJson = JsonSerializer.Serialize(metadata);

        var memoryRecords = new Collection<MemoryRecord>();

        for (var i = 0; i < chunksCount; i++)
        {
            var chunk = chunks.ElementAt(i);
            var embedding = await textEmbeddingGeneration.GenerateEmbeddingAsync(chunk, cancellationToken);
            memoryRecords.Add(MemoryRecord.LocalRecord(BuildMemoryIdentifier(memoryid, i), chunk, null, embedding, metadataJson));
        }

        await memoryStore.UpsertBatchAsync(collectionName, memoryRecords, cancellationToken).ToListAsync(cancellationToken: cancellationToken);
    }
}

