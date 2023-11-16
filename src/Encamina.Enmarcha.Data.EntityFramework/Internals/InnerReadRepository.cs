using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="ReadRepositoryBase{TEntity}"/> powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this read repository.</typeparam>
internal sealed class InnerReadRepository<TEntity> : ReadRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerReadRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerReadRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
