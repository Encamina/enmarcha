using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Extensions;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for asynchronous write repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous write repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AsyncWriteRepositoryBase<TEntity> : IAsyncWriteRepository<TEntity> where TEntity : class
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncWriteRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected AsyncWriteRepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        if (entity != null)
        {
            await dbContext.GetSet<TEntity>().AddAsync(entity, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task AddBatchAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        if (entities?.Any() ?? false)
        {
            await dbContext.GetSet<TEntity>().AddRangeAsync(entities, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public virtual async Task DeleteAsync<TEntityId>(TEntityId id, CancellationToken cancellationToken)
    {
        var set = dbContext.GetSet<TEntity>();

        var entity = await set.FindAsync(new object?[] { id }, cancellationToken);

        if (entity != null)
        {
            set.Remove(entity);
        }
    }
}
