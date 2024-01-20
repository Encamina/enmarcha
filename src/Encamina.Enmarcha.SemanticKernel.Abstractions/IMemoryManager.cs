// Ignore Spelling: Upsert

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents a memory manager (with some CRUD operations) over memories with multiple chunks from an <see cref="IMemoryStore"/>.
/// </summary>
public interface IMemoryManager
{
    /// <summary>
    /// Gets the instance of the memory store manage by this manager.
    /// </summary>
    IMemoryStore MemoryStore { get; }

    /// <summary>
    /// Upserts the memory content into a collection.
    /// </summary>
    /// <param name="memoryId">The memory unique identifier.</param>
    /// <param name="collectionName">Name of the collection where the content will be saved.</param>
    /// <param name="chunks">List of strings containing the content of the memory.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <param name="metadata">Metadata of the memory.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpsertMemoryAsync(string memoryId, string collectionName, IEnumerable<string> chunks, Kernel kernel, IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the memory content from a collection.
    /// </summary>
    /// <param name="memoryId">The memory unique identifier.</param>
    /// <param name="collectionName">Name of the collection from where the content will be deleted.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the memory content from a collection.
    /// </summary>
    /// <param name="memoryId">The memory unique identifier.</param>
    /// <param name="collectionName">Name of the collection where the content will be saved.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> containing the <see cref="MemoryContent"/>, or <see langword="null"/> if the content could not be found.</returns>
    Task<MemoryContent> GetMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken);

    /// <summary>
    /// Upserts a batch of memory contents into a collection.
    /// </summary>
    /// <param name="collectionName">Name of the collection where the content will be saved.</param>
    /// <param name="memoryContents">
    /// Dictionary with the memory contents to upsert.
    /// The <c>key</c> in the dictionary must contain a unique identifier for the content of the memory,
    /// and the <c>value</c> of the dictionary must provide the memory content (chunks and metadata).
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The unique identifiers for the memory records (not necessarily the same as the unique identifier of the memory content).</returns>
    IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, Kernel kernel, CancellationToken cancellationToken);
}
