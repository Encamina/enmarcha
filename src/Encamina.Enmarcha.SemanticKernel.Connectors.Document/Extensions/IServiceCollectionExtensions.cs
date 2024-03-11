using Encamina.Enmarcha.AI.Abstractions;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentExtractor(this IServiceCollection services)
    {
        return services.AddSingleton<IDocumentContentExtractor, DefaultDocumentContentExtractor>();
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentExtractor(this IServiceCollection services, Func<string, int> lengthFunction)
    {
        services.AddSingleton(lengthFunction);
        return services.AddDefaultDocumentContentExtractor();
    }

    /// <summary>
    /// Adds a default implementation of Semantic <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentSemanticExtractor(this IServiceCollection services)
    {
        return services.AddDefaultDocumentContentSemanticExtractor(ServiceLifetime.Singleton);
    }

    /// <summary>
    /// Adds a default implementation of Semantic <see cref="IDocumentContentExtractor"/> to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <param name="serviceLifetime">The lifetime for the registered services.</param>
    public static IServiceCollection AddDefaultDocumentContentSemanticExtractor(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        return services.AddType<IDocumentContentExtractor, DefaultDocumentContentSemanticExtractor>(serviceLifetime);
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentConnectorProvider"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectorProvider(this IServiceCollection services)
    {
        return services.AddSingleton<IDocumentConnectorProvider, DefaultDocumentContentExtractor>();
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentConnectorProvider"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectorProvider(this IServiceCollection services, Func<string, int> lengthFunction)
    {
        services.AddSingleton(lengthFunction);
        return services.AddDefaultDocumentConnectorProvider();
    }
}
