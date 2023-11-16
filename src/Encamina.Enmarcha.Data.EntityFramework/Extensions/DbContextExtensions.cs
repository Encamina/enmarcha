using Microsoft.EntityFrameworkCore;

namespace Encamina.Enmarcha.Data.EntityFramework.Extensions;

/// <summary>
/// Extension for <see cref="DbContext"/>s.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Gets a valid <see cref="DbSet{TEntity}"/> from the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of the (data) entity.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <returns>A valid <see cref="DbSet{TEntity}"/> from the <see cref="DbContext"/>.</returns>
    public static DbSet<TEntity> GetSet<TEntity>(this DbContext dbContext) where TEntity : class => dbContext.Set<TEntity>();

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> from the <see cref="DbContext"/>, with or without tracking.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of the (data) entity.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="withNoTracking">A value indicating whether the retrieved entities are with or without tracking.</param>
    /// <returns>Returns an <see cref="IQueryable{T}"/> from the <see cref="DbContext"/>.</returns>
    public static IQueryable<TEntity> AsQueryable<TEntity>(this DbContext dbContext, bool withNoTracking) where TEntity : class
        => dbContext.AsQueryable<TEntity>(withNoTracking, false);

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> from the <see cref="DbContext"/>, with or without tracking.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of the (data) entity.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/>.</param>
    /// <param name="withNoTracking">A value indicating whether the retrieved entities are with or without tracking.</param>
    /// <param name="withIdentityResolution">
    /// A value indicating whether identity resolution will be performed to ensure that all occurrences of an entity with a given key
    /// in the <see cref="IQueryable{T}"/> are represented by the same entity instance.
    /// </param>
    /// <returns>Returns an <see cref="IQueryable{T}"/> from the <see cref="DbContext"/>.</returns>
    public static IQueryable<TEntity> AsQueryable<TEntity>(this DbContext dbContext, bool withNoTracking, bool withIdentityResolution) where TEntity : class
    {
        IQueryable<TEntity> set = dbContext.GetSet<TEntity>();

        if (withNoTracking)
        {
            set = withIdentityResolution ? set.AsNoTrackingWithIdentityResolution() : set.AsNoTracking();
        }

        return set;
    }
}
