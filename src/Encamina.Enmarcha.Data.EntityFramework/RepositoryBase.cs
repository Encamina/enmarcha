using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Internals;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for a repositories powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asynchronous repository.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly IReadRepository<TEntity> readRepository;
    private readonly IWriteRepository<TEntity> writeRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected RepositoryBase(DbContext dbContext)
    {
        Guard.IsNotNull(dbContext);

        readRepository = new InnerReadRepository<TEntity>(dbContext);
        writeRepository = new InnerWriteRepository<TEntity>(dbContext);
    }

    /// <inheritdoc/>
    public virtual void Add(TEntity entity) => writeRepository.Add(entity);

    /// <inheritdoc/>
    public virtual void AddBatch(IEnumerable<TEntity> entities) => writeRepository.AddBatch(entities);

    /// <inheritdoc/>
    public virtual void Delete<TEntityId>(TEntityId id) => writeRepository.Delete(id);

    /// <inheritdoc/>
    public virtual void Delete(TEntity entity) => writeRepository.Delete(entity);

    /// <inheritdoc/>
    public virtual void DeleteBatch(IEnumerable<TEntity> entities) => writeRepository.DeleteBatch(entities);

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> GetAll() => readRepository.GetAll();

    /// <inheritdoc/>
    public virtual IQueryable<TEntity> GetAll([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction) => readRepository.GetAll(queryFunction);

    /// <inheritdoc/>
    public virtual TEntity? GetById<TEntityId>(TEntityId id) => readRepository.GetById(id);

    /// <inheritdoc/>
    public virtual void Update(TEntity entity) => writeRepository.Update(entity);
}
