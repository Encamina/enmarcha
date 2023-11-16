using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Internal implementation of <see cref="ICosmosRepositoryFactory"/>.
/// </summary>
internal sealed class CosmosRepositoryFactory : ICosmosRepositoryFactory
{
    private readonly ICosmosInitializer cosmosInitializer;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosRepositoryFactory"/> class.
    /// </summary>
    /// <param name="cosmosInitializer">A valid instance of <see cref="ICosmosInitializer"/> from dependency injection.</param>
    /// <param name="serviceProvider">
    /// A valid instance of <see cref="IServiceProvider"/> from dependency injection, used to activate instances of <see cref="ICosmosRepository{T}"/>.
    /// </param>
    public CosmosRepositoryFactory(ICosmosInitializer cosmosInitializer, IServiceProvider serviceProvider)
    {
        this.cosmosInitializer = cosmosInitializer;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public ICosmosRepository<T> Create<T>(string database, string container)
    {
        return Create<T>(cosmosInitializer.GetContainer(database, container));
    }

    /// <inheritdoc/>
    public ICosmosRepository<T> Create<T>(string container)
    {
        return Create<T>(cosmosInitializer.GetContainer(container));
    }

    private ICosmosRepository<T> Create<T>(Container container)
    {
        return ActivatorUtilities.CreateInstance<CosmosRepository<T>>(serviceProvider, container);
    }
}
