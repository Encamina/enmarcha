using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Abstractions.Internals;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure default services for OpenAI-based services.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds a default provider for factories of completion services based on OpenAI.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="ICompletionServiceFactoryProvider"/> as «Singleton».
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDefaultCompletionServiceFactoryProvider(this IServiceCollection services)
    {
        services.TryAddSingleton<ICompletionServiceFactoryProvider, CompletionServiceFactoryProvider>();
        return services;
    }
}
