using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents types that helps working with memory stores (i.e., <see cref="IMemoryStore"/>) through a memory handler (<see cref="IMemoryStoreExtender"/>),
/// when sometimes it is necessary to centrally manage how some operations or actions are done.
/// </summary>
public interface IMemoryStoreHandler
{
    /// <summary>
    /// Gets a postfix string value that can be used when creating a collection name.
    /// </summary>
    string CollectionNamePostfix { get; init; }

    /// <summary>
    /// Gets a prefix string value that can be used when creating a collection name.
    /// </summary>
    string CollectionNamePrefix { get; init; }

    /// <summary>
    /// Gets the <see cref="IMemoryManager"/> that manages the memory stored handled by this instance.
    /// </summary>
    [Obsolete("This property is obsolete and will be removed in a future version. Use the Encamina.Enmarcha.SemanticKernel.Abstractions.IMemoryStoreHandler.MemoryStoreExtender property instead.", false)]
    IMemoryManager MemoryManager { get; }

    /// <summary>
    /// Gets the <see cref="IMemoryStoreExtender"/> that extend the memory stored handled by this instance.
    /// </summary>
    IMemoryStoreExtender MemoryStoreExtender { get; }

    /// <summary>
    /// Gets the name of a collection from its unique identifier.
    /// </summary>
    /// <param name="collectionId">The unique identifier of the collection.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The name of the collection.</returns>
    Task<string> GetCollectionNameAsync(string collectionId, CancellationToken cancellationToken);
}
