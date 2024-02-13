using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.Cosmos;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure connection with a Cosmos DB.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for Cosmos DB with configuration parameters from the current configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CosmosOptions>().Bind(configuration.GetSection(nameof(CosmosOptions))).ValidateDataAnnotations().ValidateOnStart();

        ConfigureCosmos(services);

        return services;
    }

    /// <summary>
    /// Adds support for Cosmos DB with configuration options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">An action to configure the options.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCosmos(this IServiceCollection services, Action<CosmosOptions> options)
    {
        services.AddOptions<CosmosOptions>().Configure(options).ValidateDataAnnotations().ValidateOnStart();

        ConfigureCosmos(services);

        return services;
    }

    private static void ConfigureCosmos(IServiceCollection services)
    {
        services.TryAddSingleton<ICosmosInitializer, CosmosInitializer>();
        services.TryAddSingleton<ICosmosRepositoryFactory, CosmosRepositoryFactory>();

        // Repositories should be ephemeral, therefore they should be created as they are needed!
        services.TryAddScoped(typeof(ICosmosRepository<>), typeof(CosmosRepository<>));
        services.TryAddScoped(typeof(IAsyncRepository<>), typeof(CosmosRepository<>));
    }
}
