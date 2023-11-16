using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="WriteRepositoryBase{TEntity}"/> powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this write repository.</typeparam>
internal sealed class InnerWriteRepository<TEntity> : WriteRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerWriteRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerWriteRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
