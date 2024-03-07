using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.Core;
using Encamina.Enmarcha.Data.AzureAISearch;
using Encamina.Enmarcha.Data.Qdrant.Abstractions;
using Encamina.Enmarcha.Data.Qdrant.Abstractions.Extensions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Memory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure memory connectors for Semantic Kernel.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/memories/"/>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure AI Search as vector database for a memory store (<see cref="IMemoryStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension methods requires a <see cref="AzureAISearchOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureAISearchMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton<IMemoryStore>(serviceProvider =>
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AzureAISearchOptions>>();

            static AzureAISearchMemoryStore Builder(AzureAISearchOptions options)
            {
                return new AzureAISearchMemoryStore(options.Endpoint.AbsoluteUri, options.Key);
            }

            var debouncedBuilder = Debouncer.Debounce<AzureAISearchOptions>(options => Builder(options), 300);

            var memory = Builder(optionsMonitor.CurrentValue);

            optionsMonitor.OnChange(debouncedBuilder);

            return memory;
        });
    }

    /// <summary>
    /// Adds Qdrant vector database as a memory store (<see cref="IMemoryStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension methods requires a <see cref="QdrantOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQdrantMemoryStore(this IServiceCollection services)
    {
        return services.AddSingleton<IMemoryStore>(sp =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<QdrantOptions>>();

            var httpClient = new HttpClient(new HttpClientHandler()
            {
                CheckCertificateRevocationList = true,
            }, disposeHandler: false);

            static QdrantMemoryStore Builder(IServiceProvider serviceProvider, HttpClient httpClient, QdrantOptions options)
            {
                httpClient.ConfigureHttpClientForQdrant(options);

                return new QdrantMemoryStore(httpClient, options.VectorSize, loggerFactory: serviceProvider.GetService<ILoggerFactory>());
            }

            var debouncedBuilder = Debouncer.Debounce<QdrantOptions>(options => Builder(sp, httpClient, options), 300);

            var memory = Builder(sp, httpClient, optionsMonitor.CurrentValue);

            optionsMonitor.OnChange(debouncedBuilder);

            return memory;
        });
    }

    /// <summary>
    /// Adds semantic text memory (<see cref="ISemanticTextMemory"/>) to the <see cref="IServiceCollection"/> in the specified <see cref="ServiceLifetime">service lifetime</see>.
    /// </summary>
    /// <remarks>
    /// By default, the <see cref="ServiceLifetime">service lifetime</see> is <see cref="ServiceLifetime.Singleton"/>.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the semantic text memory.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddSemanticTextMemory(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureOpenAIOptions>>().Value;

            return new MemoryBuilder()
                .WithAzureOpenAITextEmbeddingGeneration(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                .WithMemoryStore(sp.GetRequiredService<IMemoryStore>())
                .Build();
        });
    }

    /// <summary>
    /// Adds Azure AI Search as a named vector database for a memory store (<see cref="IMemoryStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension methods requires a <see cref="AzureAISearchOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="memoryProviderName">name of the memory provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureAISearchNamedMemoryStore(this IServiceCollection services, string memoryProviderName)
    {
        return services.AddKeyedSingleton<IMemoryStore>(memoryProviderName, (serviceProvider, k) =>
        {
            var optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<AzureAISearchOptions>>();

            static AzureAISearchMemoryStore Builder(AzureAISearchOptions options)
            {
                return new AzureAISearchMemoryStore(options.Endpoint.AbsoluteUri, options.Key);
            }

            var debouncedBuilder = Debouncer.Debounce<AzureAISearchOptions>(options => Builder(options), 300);

            var memory = Builder(optionsMonitor.CurrentValue);

            optionsMonitor.OnChange(debouncedBuilder);

            return memory;
        });
    }

    /// <summary>
    /// Adds Qdrant vector database as a named memory store (<see cref="IMemoryStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension methods requires a <see cref="QdrantOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="memoryProviderName">name of the memory provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQdrantNamedMemoryStore(this IServiceCollection services, string memoryProviderName)
    {
        return services.AddKeyedSingleton<IMemoryStore>(memoryProviderName, (sp, k) =>
        {
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<QdrantOptions>>();

            var httpClient = new HttpClient(new HttpClientHandler()
            {
                CheckCertificateRevocationList = true,
            }, disposeHandler: false);

            static QdrantMemoryStore Builder(IServiceProvider serviceProvider, HttpClient httpClient, QdrantOptions options)
            {
                httpClient.ConfigureHttpClientForQdrant(options);

                return new QdrantMemoryStore(httpClient, options.VectorSize, loggerFactory: serviceProvider.GetService<ILoggerFactory>());
            }

            var debouncedBuilder = Debouncer.Debounce<QdrantOptions>(options => Builder(sp, httpClient, options), 300);

            var memory = Builder(sp, httpClient, optionsMonitor.CurrentValue);

            optionsMonitor.OnChange(debouncedBuilder);

            return memory;
        });
    }
}
