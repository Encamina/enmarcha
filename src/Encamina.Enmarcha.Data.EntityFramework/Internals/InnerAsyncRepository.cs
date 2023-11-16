using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="AsyncRepositoryBase{TEntity}"/> powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous repository.</typeparam>
internal class InnerAsyncRepository<TEntity> : AsyncRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerAsyncRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerAsyncRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
