namespace Encamina.Enmarcha.Data.Abstractions;

/// <summary>
/// Represents an asynchronous repository with operations to read and write entities.
/// </summary>
/// <typeparam name="TEntity">The specific type of (data) entity handled by this asynchronous repository.</typeparam>
public interface IAsyncRepository<TEntity> : IAsyncReadRepository<TEntity>, IAsyncWriteRepository<TEntity>
{
}
