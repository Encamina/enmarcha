using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Internals;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for a unit of work that can manage both synchronous and asynchronous operations powered by Entity Framework.
/// </summary>
/// <remarks>
/// A unit of work is referred to as a single transaction that involves multiple get, insert, update, or delete operations.
/// In simple terms, it means that for a specific action (such as a registration on a website for eqxmple), all the get, insert,
/// update, and delete operations are handled as a single transaction. This is more efficient than handling multiple transactions in a chattier way.
/// </remarks>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class FullUnitOfWork : IFullUnitOfWork
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IAsyncUnitOfWork unitOfWorkAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected FullUnitOfWork(DbContext dbContext)
    {
        unitOfWork = new InnerUnitOfWork(dbContext);
        unitOfWorkAsync = new InnerAsyncUnitOfWork(dbContext);
    }

    /// <inheritdoc/>
    public virtual void Save() => unitOfWork.Save();

    /// <inheritdoc/>
    public virtual async Task SaveAsync(CancellationToken cancellationToken) => await unitOfWorkAsync.SaveAsync(cancellationToken);

    /// <inheritdoc/>
    public virtual IAsyncRepository<TEntity> GetAsyncRepository<TEntity>() where TEntity : class => unitOfWorkAsync.GetAsyncRepository<TEntity>();

    /// <inheritdoc/>
    public virtual IRepository<TEntity> GetRepository<TEntity>() where TEntity : class => unitOfWork.GetRepository<TEntity>();
}
