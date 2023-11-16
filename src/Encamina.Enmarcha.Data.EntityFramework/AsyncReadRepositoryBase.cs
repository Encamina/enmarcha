using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Extensions;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for asynchronous read repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous read repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AsyncReadRepositoryBase<TEntity> : IAsyncReadRepository<TEntity> where TEntity : class
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncReadRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected AsyncReadRepositoryBase(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        => (await dbContext.GetSet<TEntity>().ToListAsync(cancellationToken)).AsQueryable();

    /// <inheritdoc/>
    public virtual async Task<IQueryable<TEntity>> GetAllAsync([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(queryFunction);

        return (await queryFunction(dbContext.GetSet<TEntity>()).ToListAsync(cancellationToken)).AsQueryable();
    }

    /// <inheritdoc/>
    public virtual async Task<TEntity> GetByIdAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken)
        => await dbContext.GetSet<TEntity>().FindAsync(new object[] { id }, cancellationToken);
}
