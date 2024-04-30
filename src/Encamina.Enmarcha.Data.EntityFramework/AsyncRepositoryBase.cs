using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Internals;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for asynchronous repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asynchronous repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AsyncRepositoryBase<TEntity> : IAsyncRepository<TEntity> where TEntity : class
{
    private readonly IAsyncReadRepository<TEntity> readRepository;
    private readonly IAsyncWriteRepository<TEntity> writeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected AsyncRepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        readRepository = new InnerAsyncReadRepository<TEntity>(dbContext);
        writeRepository = new InnerAsyncWriteRepository<TEntity>(dbContext);
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken) => await writeRepository.AddAsync(entity, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task AddBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) => await writeRepository.AddBatchAsync(entities, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task DeleteAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await writeRepository.DeleteAsync(id, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken) => await readRepository.GetAllAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction, CancellationToken cancellationToken)
        => await readRepository.GetAllAsync(queryFunction, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<TEntity?> GetByIdAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken) => await readRepository.GetByIdAsync(id, cancellationToken);
}
