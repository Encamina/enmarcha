using Azure.Core;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.Cosmos;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

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

        services.TryAddSingleton<ICosmosInitializer, CosmosInitializer>();

        ConfigureCosmosRepository(services);

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

        services.TryAddSingleton<ICosmosInitializer, CosmosInitializer>();

        ConfigureCosmosRepository(services);

        return services;
    }

    /// <summary>
    /// Adds support for Cosmos DB with configuration parameters from the current configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="tokenCredential">The <see cref="TokenCredential"/> to use for authenticating with Cosmos DB.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration, TokenCredential tokenCredential)
    {
        services.AddOptions<CosmosOptions>().Bind(configuration.GetSection(nameof(CosmosOptions))).ValidateDataAnnotations().ValidateOnStart();

        services.TryAddSingleton<ICosmosInitializer>(sp => new CosmosInitializer(sp.GetRequiredService<IOptions<CosmosOptions>>(), tokenCredential));

        ConfigureCosmosRepository(services);

        return services;
    }

    /// <summary>
    /// Adds support for Cosmos DB with configuration parameters from the current configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="tokenCredentialProvider">The function to provide a <see cref="TokenCredential"/> for authenticating with Cosmos DB.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCosmos(this IServiceCollection services, IConfiguration configuration, Func<IServiceProvider, TokenCredential> tokenCredentialProvider)
    {
        services.AddOptions<CosmosOptions>().Bind(configuration.GetSection(nameof(CosmosOptions))).ValidateDataAnnotations().ValidateOnStart();

        services.TryAddSingleton<ICosmosInitializer>(sp => new CosmosInitializer(sp.GetRequiredService<IOptions<CosmosOptions>>(), tokenCredentialProvider(sp)));

        ConfigureCosmosRepository(services);

        return services;
    }

    private static void ConfigureCosmosRepository(IServiceCollection services)
    {
        services.TryAddSingleton<ICosmosRepositoryFactory, CosmosRepositoryFactory>();

        // Repositories should be ephemeral, therefore they should be created as they are needed!
        services.TryAddScoped(typeof(ICosmosRepository<>), typeof(CosmosRepository<>));
        services.TryAddScoped(typeof(IAsyncRepository<>), typeof(CosmosRepository<>));
    }
}
