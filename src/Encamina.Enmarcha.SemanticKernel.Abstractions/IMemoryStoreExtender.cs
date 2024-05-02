// Ignore Spelling: Upsert

using Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents a memory manager (with some CRUD operations) over memories with multiple chunks from an <see cref="IMemoryStore"/>.
/// </summary>
public interface IMemoryStoreExtender
{
    /// <summary>
    /// This event is fired when <see cref="IMemoryStore"/> executes an action.
    /// The event information is defined in the object <see cref="MemoryStoreEventArgs"/>, and the action types in the object <see cref="MemoryStoreEventTypes"/>.
    /// </summary>
    event EventHandler<MemoryStoreEventArgs> MemoryStoreEvent;

    /// <summary>
    /// Gets the instance of the <see cref="IMemoryStore"/> object for this extender class.
    /// </summary>
    IMemoryStore MemoryStore { get; }

    /// <summary>
    /// This action fired a <see cref="MemoryStoreEvent"/> event when is invoked.
    /// </summary>
    /// <param name="e">The object <see cref="MemoryStoreEventArgs"/> contains the event arguments for this event.</param>
    void RaiseMemoryStoreEvent(MemoryStoreEventArgs e);

    /// <summary>
    /// Upserts the memory content into a collection.
    /// </summary>
    /// <param name="memoryId">The memory unique identifier.</param>
    /// <param name="collectionName">Name of the collection where the content will be saved.</param>
    /// <param name="chunks">List of strings containing the content of the memory.</param>
    /// <param name="kernel"><see cref="Kernel"/> instance object.</param>
    /// <param name="metadata">Metadata of the memory.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task UpsertMemoryAsync(
        string memoryId,
        string collectionName,
        IEnumerable<string> chunks,
        Kernel kernel,
        IDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

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
    /// <param name="collectionName">Name of the collection where the content will be retrieved.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> containing the <see cref="MemoryContent"/>, or <see langword="null"/> if the content could not be found.</returns>
    Task<MemoryContent?> GetMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if the memory exists in a collection.
    /// </summary>
    /// <param name="memoryId">The memory unique identifier.</param>
    /// <param name="collectionName">Name of the collection where the memoryId will be searched.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns><c>true</c> if the memory exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsMemoryAsync(string memoryId, string collectionName, CancellationToken cancellationToken);

    /// <summary>
    /// Upserts a batch of memory contents into a collection.
    /// </summary>
    /// <param name="collectionName">Name of the collection where the content will be saved.</param>
    /// <param name="memoryContents">
    /// Dictionary with the memory contents to upsert.
    /// The <c>key</c> in the dictionary must contain a unique identifier for the content of the memory,
    /// and the <c>value</c> of the dictionary must provide the memory content (chunks and metadata).
    /// </param>
    /// <param name="kernel"><see cref="Kernel"/> object instance.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The unique identifiers for the memory records (not necessarily the same as the unique identifier of the memory content).</returns>
    IAsyncEnumerable<string> BatchUpsertMemoriesAsync(string collectionName, IDictionary<string, MemoryContent> memoryContents, Kernel kernel, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IMemoryStoreExtender" />
[Obsolete("This interface is obsolete and will be removed in a future version. Use the Encamina.Enmarcha.SemanticKernel.Abstractions.IMemoryStoreExtender interface instead.", false)]
public interface IMemoryManager : IMemoryStoreExtender
{
}