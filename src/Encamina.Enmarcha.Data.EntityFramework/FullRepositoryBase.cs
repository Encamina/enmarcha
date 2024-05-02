using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Internals;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for a repository with both synchronous and asynchronous operations to read and write entities powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class FullRepositoryBase<TEntity> : IFullRepository<TEntity> where TEntity : class
{
    private readonly IRepository<TEntity> repository;
    private readonly IAsyncRepository<TEntity> repositoryAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected FullRepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        repository = new InnerRepository<TEntity>(dbContext);
        repositoryAsync = new InnerAsyncRepository<TEntity>(dbContext);
    }

    /// <inheritdoc/>
    public virtual void Add(TEntity entity) => repository.Add(entity);

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken) => await repositoryAsync.AddAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public virtual void AddBatch(IEnumerable<TEntity> entities) => repository.AddBatch(entities);

    /// <inheritdoc/>
    public virtual async Task AddBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) => await repositoryAsync.AddBatchAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public virtual void Delete<TEntityId>(TEntityId id) => repository.Delete(id);

    /// <inheritdoc/>
    public virtual void Delete(TEntity entity) => repository.Delete(entity);

    /// <inheritdoc/>
    public virtual async Task DeleteAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await repositoryAsync.DeleteAsync(id, cancellationToken);

    /// <inheritdoc/>
    public virtual void DeleteBatch(IEnumerable<TEntity> entities) => repository.DeleteBatch(entities);

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> GetAll() => repository.GetAll();

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> GetAll([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction) => repository.GetAll(queryFunction);

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken) => await repositoryAsync.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction, CancellationToken cancellationToken)
        => await repositoryAsync.GetAllAsync(queryFunction, cancellationToken);

    /// <inheritdoc/>
    public virtual TEntity? GetById<TEntityId>(TEntityId id) => repository.GetById(id);

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await repositoryAsync.GetByIdAsync(id, cancellationToken);

    /// <inheritdoc/>
    public virtual void Update(TEntity entity) => repository.Update(entity);
}
