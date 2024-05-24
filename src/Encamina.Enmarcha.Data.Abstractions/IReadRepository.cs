using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a repository pattern specifically for read operations.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this read repository.</typeparam>
public interface IReadRepository<TEntity>
{
    /// <summary>
    /// Gets all entities from the repository.
    /// </summary>
    /// <returns>All the entities in the repository.</returns>
    IQueryable<TEntity> GetAll();

    /// <summary>
    /// Gets all entities from the repository that satisfies a given query.
    /// </summary>
    /// <param name="queryFunction">A function that represents a query to apply on the entities.</param>
    /// <returns>All the entities in the repository that satisfies the given query.</returns>
    IQueryable<TEntity> GetAll([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction);

    /// <summary>
    /// Gets an entity from the repository given its unique identifier.
    /// </summary>
    /// <typeparam name="TEntityId">The specific type of the entity's unique identifier.</typeparam>
    /// <param name="id">The entity's unique identifier.</param>
    /// <returns>An entity from the repository.</returns>
    TEntity? GetById<TEntityId>(TEntityId id);
}
