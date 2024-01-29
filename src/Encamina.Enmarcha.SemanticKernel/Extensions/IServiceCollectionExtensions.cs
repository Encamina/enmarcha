using Encamina.Enmarcha.SemanticKernel;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.SemanticKernel;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the <see cref="MemoryStoreExtender"/> type as transient service instance of the <see cref="IMemoryStoreExtender"/> service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This method registers the <see cref="MemoryStoreExtender"/> type as a transient service due to its dependency on <see cref="Kernel"/>, which is typically registered as transient or scoped (rarely as singleton).
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    [Obsolete("This extension is obsolete and will be removed in a future version. Use the Microsoft.Extensions.DependencyInjection.IServiceCollectionExtensions.AddMemoryStoreExtender() extension instead.", false)]
    public static IServiceCollection AddMemoryManager(this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryManager, MemoryManager>();

        return services;
    }

    /// <summary>
    /// Adds and configures the <see cref="MemoryStoreExtender"/> type as transient service instance of the <see cref="IMemoryStoreExtender"/> service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This method registers the <see cref="MemoryStoreExtender"/> type as a transient service due to its dependency on <see cref="Kernel"/>, which is typically registered as transient or scoped (rarely as singleton).
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMemoryStoreExtender(this IServiceCollection services)
    {
        services.TryAddSingleton<IMemoryStoreExtender, MemoryStoreExtender>();

        return services;
    }

    /// <summary>
    /// Adds and configures an «Ephemeral Memory Store Handler», which removes collections from memory after a configured time of inactivity (thus ephemeral), as a transient
    /// instance of the <see cref="IMemoryStoreHandler"/> service to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>
    /// This method registers the <see cref="EphemeralMemoryStoreHandlerOptions"/> type as a transient service because of its dependency on <see cref="IMemoryStoreExtender"/>s, which depend on <see cref="Kernel"/>, typically a
    /// transient or scoped service (rarely a singleton).
    /// </remarks>
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
