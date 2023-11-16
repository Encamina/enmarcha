using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a unit of work.
/// </summary>
/// <remarks>
/// A unit of work is referred to as a single transaction that involves multiple get, insert, update, or delete operations.
/// In simple terms, it means that for a specific action (such as a registration on a website for eqxmple), all the get, insert,
/// update, and delete operations are handled as a single transaction. This is more efficient than handling multiple transactions in a chattier way.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets a repository to handled read and write operations on a specific type of entity.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of (data) entity handled by the repository.</typeparam>
    /// <returns>A valid instance of a repository to handled read and write operations on a specific type of entity.</returns>
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves all changes made.
    /// </summary>
    void Save();
}
