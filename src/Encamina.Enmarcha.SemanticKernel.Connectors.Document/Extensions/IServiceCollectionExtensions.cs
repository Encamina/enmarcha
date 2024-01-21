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
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentContentExtractor(this IServiceCollection services, Func<string, int> lengthFunction = null)
    {
        if (lengthFunction is not null)
        {
            services.AddSingleton(lengthFunction);
        }

        return services.AddSingleton<IDocumentContentExtractor, DefaultDocumentContentExtractor>();
    }

    /// <summary>
    /// Adds a default implementation of <see cref="IDocumentConnectorProvider"/> to the specified <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultDocumentConnectorProvider(this IServiceCollection services, Func<string, int> lengthFunction = null)
    {
        if (lengthFunction is not null)
        {
            services.AddSingleton(lengthFunction);
        }

        services.AddSingleton<IDocumentConnectorProvider, DefaultDocumentContentExtractor>();

        return services;
    }
}
