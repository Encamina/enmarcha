using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.EntityFramework.Internals;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework;

/// <summary>
/// Base class for an asynchronous unit of work powered by Entity Framework.
/// </summary>
/// <remarks>
/// A unit of work is referred to as a single transaction that involves multiple get, insert, update, or delete operations.
/// In simple terms, it means that for a specific action (such as a registration on a website for eqxmple), all the get, insert,
/// update, and delete operations are handled as a single transaction. This is more efficient than handling multiple transactions in a chattier way.
/// </remarks>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AsyncUnitOfWorkBase : IAsyncUnitOfWork
{
    private readonly DbContext dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncUnitOfWorkBase"/> class.
    /// </summary>
    /// <param name="dbContext">The instance of a <see cref="DbContext"/> to use as connection with Entity Framework.</param>
    protected AsyncUnitOfWorkBase(DbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    /// <inheritdoc/>
    public virtual IAsyncRepository<TEntity> GetAsyncRepository<TEntity>() where TEntity : class
    {
        return new InnerAsyncRepository<TEntity>(dbContext);
    }

    /// <inheritdoc/>
    public async virtual Task SaveAsync(CancellationToken cancellationToken) => await dbContext.SaveChangesAsync(cancellationToken);
}
