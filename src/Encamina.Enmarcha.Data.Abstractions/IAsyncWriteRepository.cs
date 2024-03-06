using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents an asynchronous repository pattern specifically for write operations.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asynchronous write repository.</typeparam>
public interface IAsyncWriteRepository<in TEntity>
{
    /// <summary>
    /// Adds an entity asynchronously into the repository.
    /// </summary>
    /// <param name="entity">The entity to add into the repository.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple entities asynchronously into the repository, as a batch operation.
    /// </summary>
    /// <param name="entities">The entities to add into the repository.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task AddBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity asynchronously from the repository given its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <typeparam name="TEntityId">The specific type of the entity's unique identifier.</typeparam>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task DeleteAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken);
}
