using static System.Net.WebRequestMethods;

namespace Encamina.Enmarcha.Data.Qdrant.Abstractions;

/// <summary>
/// <para>
/// The vector database Qdrant supports creating snapshots, which consist in a <c>.tar</c> archive file containing the necessary
/// data to restore the collection at the time of the snapshot. Snapshots are performed on a per-collection basis.
/// </para>
/// <para>
/// This interface represents handlers that can help with the creation of snapshots.
/// </para>
/// </summary>
/// <seealso href="https://qdrant.tech/documentation/concepts/snapshots/"/>
public interface IQdrantSnapshotHandler
{
    /// <summary>
    /// Creates a snapshot of the specified collection.
    /// </summary>
    /// <param name="collectionName">Collection name to take a snapshot of.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous snapshot creation.</returns>
    Task CreateCollectionSnapshotAsync(string collectionName, CancellationToken cancellationToken);
}
