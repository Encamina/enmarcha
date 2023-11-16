using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Extensions;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for write repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous write repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class WriteRepositoryBase<TEntity> : IWriteRepository<TEntity> where TEntity : class
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected WriteRepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public virtual void Add(TEntity entity) => dbContext.GetSet<TEntity>().Add(entity);

    /// <inheritdoc/>
    public virtual void AddBatch(IEnumerable<TEntity> entities)
    {
        if (entities?.Any() ?? false)
        {
            dbContext.GetSet<TEntity>().AddRange(entities);
        }
    }

    /// <inheritdoc/>
    public virtual void Delete<TEntityId>(TEntityId id)
    {
        var set = dbContext.GetSet<TEntity>();

        var entity = set.Find(id);

        if (entity != null)
        {
            set.Remove(entity);
        }
    }

    /// <inheritdoc/>
    public virtual void Delete(TEntity entity) => dbContext.GetSet<TEntity>().Remove(entity);

    /// <inheritdoc/>
    public virtual void DeleteBatch(IEnumerable<TEntity> entities) => dbContext.GetSet<TEntity>().RemoveRange(entities);

    /// <inheritdoc/>
    public virtual void Update(TEntity entity) => dbContext.GetSet<TEntity>().Update(entity);
}
