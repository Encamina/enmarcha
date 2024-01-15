using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents types that helps working with memory stores (i.e., <see cref="IMemoryStore"/>), when sometimes
/// it is necessary to centrally manage how some operations or actions are done.
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
    /// Gets the name of a collection from its unique identifier.
    /// </summary>
    /// <param name="collectionId">The unique identifier of the collection.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The name of the collection.</returns>
    Task<string> GetCollectionNameAsync(string collectionId, CancellationToken cancellationToken);
}
