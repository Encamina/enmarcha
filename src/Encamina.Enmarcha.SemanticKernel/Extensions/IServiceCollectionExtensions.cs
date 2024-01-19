using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the <see cref="MemoryManager"/> type as an scoped instance of the <see cref="IMemoryManager"/> service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMemoryManager(this IServiceCollection services)
    {
        services.TryAddScoped<IMemoryManager, MemoryManager>();

        return services;
    }

    /// <summary>
    /// Adds and configures an «Ephemeral Memory Store Handler», which removes collections from memory after a configured time of inactivity (thus ephemeral), as a singleton
    /// instance of the <see cref="IMemoryStoreHandler"/> service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEphemeralMemoryStoreHandler(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<EphemeralMemoryStoreHandlerOptions>()
                .Bind(configuration.GetSection(nameof(EphemeralMemoryStoreHandlerOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.TryAddSingleton<IMemoryStoreHandler, EphemeralMemoryStoreHandler>();

        return services;
    }
}
