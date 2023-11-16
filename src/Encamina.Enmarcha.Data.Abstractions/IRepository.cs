using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents a repository with operations to read and write entities.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this repository.</typeparam>
public interface IRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
{
}
