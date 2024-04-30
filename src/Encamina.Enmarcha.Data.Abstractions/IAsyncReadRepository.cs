using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents an asynchronous repository pattern specifically for read operations.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous read repository.</typeparam>
public interface IAsyncReadRepository<TEntity>
{
    /// <summary>
    /// Gets all entities from the repository asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>All the entities in the repository.</returns>
    Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets all entities from the repository asynchronously that satisfies a given query.
    /// </summary>
    /// <param name="queryFunction">A function that represents a query to apply on the entities.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>All the entities in the repository that satisfies the given query.</returns>
    Task<IQueryable<TEntity>> GetAllAsync([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction, CancellationToken cancellationToken);

    /// <summary>
    /// Gets an entity from the repository asynchronously given its unique identifier.
    /// </summary>
    /// <typeparam name="TEntityId">The specific type of the entity's unique identifier.</typeparam>
    /// <param name="id">The entity's unique identifier.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An entity from the repository.</returns>
    Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken);
}
