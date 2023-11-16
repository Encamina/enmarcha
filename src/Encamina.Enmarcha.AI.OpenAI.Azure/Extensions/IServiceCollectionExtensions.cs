using Azure;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Azure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure OpenAI services provided by Azure.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for GPT-3 provided by Azure using current configuration.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="ICompletionServiceFactory"/> as «Singleton» and an <see cref="ICompletionService"/> as «Transient»,
    /// including any related service to the service collection and configures the connection to Azure.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureOpenAICompletionService(this IServiceCollection services, IConfiguration configuration)
    {
        return services.InnerAddAzureOpenAICompletionService(optionBuilder => optionBuilder.Bind(configuration?.GetSection(nameof(CompletionServiceOptions))).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds support for GPT-3 provided by Azure using given options.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="ICompletionServiceFactory"/> as «Singleton» and an <see cref="ICompletionService"/> as «Transient»,
    /// including any related service to the service collection and configures the connection to Azure.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">Action to configure options for the the completion service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureOpenAICompletionService(this IServiceCollection services, Action<CompletionServiceOptions> options)
    {
        return services.InnerAddAzureOpenAICompletionService(optionBuilder => optionBuilder.Configure(options).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds support for GPT-3 provided by Azure using both current configuration and given options.
    /// </summary>
    /// <remarks>
    /// Adds the <see cref="ICompletionServiceFactory"/> as «Singleton» and an <see cref="ICompletionService"/> as «Transient»,
    /// including any related service to the service collection and configures the connection to Azure.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="options">Action to configure and modify options from <paramref name="configuration"/> for the completion service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureOpenAICompletionService(this IServiceCollection services, IConfiguration configuration, Action<CompletionServiceOptions> options)
    {
        return services.InnerAddAzureOpenAICompletionService(optionBuilder => optionBuilder.Bind(configuration?.GetSection(nameof(CompletionServiceOptions))).PostConfigure(options).ValidateDataAnnotations().ValidateOnStart());
    }

    private static IServiceCollection InnerAddAzureOpenAICompletionService(this IServiceCollection services, Action<OptionsBuilder<CompletionServiceOptions>> setupOptions)
    {
        // Use an inner service collection to prevent conflicts with multiple injections of this service.
        // Also, to prevent exposing non-required or non-related services.
        var innerServiceCollection = new ServiceCollection();

        // Set-up options...
        setupOptions.Invoke(innerServiceCollection.AddOptions<CompletionServiceOptions>());

        CompletionService ServiceInstanceBuilder(IServiceProvider sp)
        {
            var innerServiceProvider = innerServiceCollection.BuildServiceProvider();
            return new CompletionService(innerServiceProvider.GetRequiredService<IOptionsMonitor<CompletionServiceOptions>>());
        }

        services.TryAddSingleton(ServiceInstanceBuilder);
        services.TryAddSingleton<ICompletionService>((Func<IServiceProvider, CompletionService>)ServiceInstanceBuilder);

        return services;
    }
}
