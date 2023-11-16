using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a repository with both synchronous and asynchronous operations to read and write entities.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this repository.</typeparam>
public interface IFullRepository<TEntity> : IRepository<TEntity>, IAsyncRepository<TEntity>
{
}
