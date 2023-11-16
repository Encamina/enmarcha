using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents an asychronous repository with operations to read and write entities.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this asychronous repository.</typeparam>
public interface IAsyncRepository<TEntity> : IAsyncReadRepository<TEntity>, IAsyncWriteRepository<TEntity>
{
}
