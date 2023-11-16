using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a repository pattern specifically for write operations.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this write repository.</typeparam>
public interface IWriteRepository<in TEntity>
{
    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add into the repository.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the repository, as a batch operation.
    /// </summary>
    /// <param name="entities">The entities to add into the repository.</param>
    void AddBatch(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository given its unique identifier.
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entity's unique identifier.</typeparam>
    /// <param name="id">The unique identifier of the entity to delete from the repository.</param>
    void Delete<TEntityId>(TEntityId id);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete from the repository.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes multiple entities from the repository, as a batch operation.
    /// </summary>
    /// <param name="entities">The entities to delete from the repository.</param>
    void DeleteBatch(IEnumerable<TEntity> entities);
}
