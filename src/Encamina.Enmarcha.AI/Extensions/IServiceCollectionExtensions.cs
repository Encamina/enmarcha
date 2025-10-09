using Encamina.Enmarcha.AI;
using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.TextSplitters;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up <see cref="ICognitiveServiceProvider"/> services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default cognitive service provider to the <see cref="IServiceCollection"/> as singleton.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultCognitiveServiceProvider(this IServiceCollection services)
    {
        return services.AddCognitiveServiceProvider<DefaultCognitiveServiceProvider>();
    }

    /// <summary>
    /// Adds a given cognitive service provider type to the <see cref="IServiceCollection"/> as singleton.
    /// </summary>
    /// <typeparam name="TCognitiveServiceProvider">The type of the cognitive service provider.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCognitiveServiceProvider<TCognitiveServiceProvider>(this IServiceCollection services)
        where TCognitiveServiceProvider : class, ICognitiveServiceProvider
    {
        services.TryAddSingleton<TCognitiveServiceProvider>();
        services.TryAddSingleton<ICognitiveServiceProvider>(serviceProvider => serviceProvider.GetRequiredService<TCognitiveServiceProvider>());

        return services;
    }

    /// <summary>
    /// Adds a «Recursive Character Text Splitter» service as singleton instance of <see cref="ITextSplitter"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddRecursiveCharacterTextSplitter(this IServiceCollection services)
    {
        return services.AddSingleton<ITextSplitter, RecursiveCharacterTextSplitter>();
    }

    /// <summary>
    /// Adds an «Enriched Recursive Character Text Splitter» service as singleton instance of <see cref="IEnrichedTextSplitter"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddEnrichedRecursiveCharacterTextSplitter(this IServiceCollection services)
    {
        return services.AddSingleton<IEnrichedTextSplitter, EnrichedRecursiveCharacterTextSplitter>();
    }

    /// <summary>
    /// Adds a «Semantic Text Splitter» service as singleton instance of <see cref="ISemanticTextSplitter"/> to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSemanticTextSplitter(this IServiceCollection services)
    {
        return services.AddSingleton<ISemanticTextSplitter, SemanticTextSplitter>();
    }
}
