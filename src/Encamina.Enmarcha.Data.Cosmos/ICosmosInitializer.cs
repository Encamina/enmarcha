using Microsoft.Azure.Cosmos;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Represents an Azure Cosmos DB connection initializer to retrieve <see cref="Container"/>s.
/// </summary>
public interface ICosmosInitializer : IDisposable
{
    /// <summary>
    /// Gets a container given its name.
    /// </summary>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A container.</returns>
    Container GetContainer(string containerName);

    /// <summary>
    /// Gets a container given database and container names.
    /// </summary>
    /// <param name="database">The name of the database.</param>
    /// <param name="containerName">The name of the container.</param>
    /// <returns>A container.</returns>
    Container GetContainer(string database, string containerName);
}
