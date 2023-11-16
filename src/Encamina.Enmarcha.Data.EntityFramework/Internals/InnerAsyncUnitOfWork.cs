using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="AsyncUnitOfWorkBase"/> powered by Entity Framework.
/// </summary>
internal class InnerAsyncUnitOfWork : AsyncUnitOfWorkBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerAsyncUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerAsyncUnitOfWork(DbContext dbContext) : base(dbContext)
    {
    }
}
