using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents an asynchronous unit of work.
/// </summary>
/// <remarks>
/// An asynchronous unit of work is referred to as a single asynchronou transaction that involves multiple get, insert, update, or delete operations.
/// In simple terms, it means that for a specific action (such as a registration on a website for eqxmple), all the get, insert,
/// update, and delete operations are handled asynchronously as a single transaction. This is more efficient than handling multiple transactions in a chattier way.
/// </remarks>
public interface IAsyncUnitOfWork
{
    /// <summary>
    /// Gets an asynchronous repository to handled read and write asynchronous operations on a specific type of entity.
    /// </summary>
    /// <typeparam name="TEntity">The specific type of (data) entity handled by the asynchronous repository.</typeparam>
    /// <returns>A valid instance of an asynchronous repository to handled read and write asynchonous operations on a specific type of entity.</returns>
    IAsyncRepository<TEntity> GetAsyncRepository<TEntity>() where TEntity : class;

    /// <summary>
    /// Saves all changes made asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task SaveAsync(CancellationToken cancellationToken);
}
