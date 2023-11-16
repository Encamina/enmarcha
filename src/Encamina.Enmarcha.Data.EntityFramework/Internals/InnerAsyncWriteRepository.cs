using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Internals;

/// <summary>
/// Internal implementation of <see cref="AsyncWriteRepositoryBase{TEntity}"/> powered by Entity Framework.
/// </summary>
/// <typeparam name="TEntity">The type of (data) entity handled by this asychronous write repository.</typeparam>
internal sealed class InnerAsyncWriteRepository<TEntity> : AsyncWriteRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InnerAsyncWriteRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">A <see cref="DbContext"/>.</param>
    internal InnerAsyncWriteRepository(DbContext dbContext) : base(dbContext)
    {
    }
}
