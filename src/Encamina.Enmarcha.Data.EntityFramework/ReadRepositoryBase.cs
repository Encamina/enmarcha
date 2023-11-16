using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Extensions;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for a read repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this read repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class ReadRepositoryBase<TEntity> : IReadRepository<TEntity> where TEntity : class
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadRepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected ReadRepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetAll()
    {
        var entities = dbContext.GetSet<TEntity>().ToList(); // Enumerate to retrieve entities from the repository...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public IQueryable<TEntity> GetAll([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction)
    {
        var entities = queryFunction(dbContext.GetSet<TEntity>()).ToList(); // Enumerate to evaluate the query function and retrieve entities from the repository...
        return entities.AsQueryable();
    }

    /// <inheritdoc/>
    public TEntity GetById<TEntityId>(TEntityId id) => dbContext.GetSet<TEntity>().Find(id);
}
