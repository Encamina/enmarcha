namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Represents a factory for Azure Cosmos DB repositories.
/// </summary>
public interface ICosmosRepositoryFactory
{
    /// <summary>
    /// Creates a valid <see cref="ICosmosRepository{T}"/> for the given container.
    /// </summary>
    /// <typeparam name="T">The type of entity to handle in the repository.</typeparam>
    /// <param name="container">The name of the container.</param>
    /// <returns>A valid <see cref="ICosmosRepository{T}"/> for the given container.</returns>
    ICosmosRepository<T> Create<T>(string container);

    /// <summary>
    /// Creates a valid <see cref="ICosmosRepository{T}"/> for the given database and container.
    /// </summary>
    /// <typeparam name="T">The type of entity to handle in the repository.</typeparam>
    /// <param name="database">The name of the database.</param>
    /// <param name="container">The name of the container in the <paramref name="database"/>.</param>
    /// <returns>A valid <see cref="ICosmosRepository{T}"/> for the given database and container.</returns>
    ICosmosRepository<T> Create<T>(string database, string container);
}
