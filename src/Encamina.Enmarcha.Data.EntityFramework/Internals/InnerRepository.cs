using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="RepositoryBase{TEntity}"/> powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this repository.</typeparam>
internal sealed class InnerRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
