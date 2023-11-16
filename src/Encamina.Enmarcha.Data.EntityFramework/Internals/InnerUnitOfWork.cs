using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="UnitOfWorkBase"/> powered by Entity Framework.
/// </summary>
internal class InnerUnitOfWork : UnitOfWorkBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerUnitOfWork"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerUnitOfWork(DbContext dbContext) : base(dbContext)
    {
    }
}
