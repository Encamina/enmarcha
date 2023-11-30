using Encamina.Enmarcha.Data.Qdrant.Abstractions;
using Encamina.Enmarcha.Data.Qdrant.Abstractions.Extensions;

using Encamina.Enmarcha.SemanticKernel.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.Connectors.Memory.Qdrant;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure memory connectors for Semantic Kernel.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/memories/"/>
public static class IServiceCollectionExtensions
{
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
            var qdrantOptionsMonitor = sp.GetRequiredService<IOptionsMonitor<QdrantOptions>>();
            var qdrantOptions = qdrantOptionsMonitor.CurrentValue;

            var httpClientHandler = new HttpClientHandler()
            {
                CheckCertificateRevocationList = true,
            };

            var httpClient = new HttpClient(httpClientHandler, disposeHandler: false).ConfigureHttpClientForQdrant(qdrantOptions);

            qdrantOptionsMonitor.OnChange(newOptions =>
            {
                httpClient.ConfigureHttpClientForQdrant(qdrantOptions);
            });

            return new QdrantMemoryStore(httpClient, qdrantOptions.VectorSize, loggerFactory: sp.GetService<ILoggerFactory>());
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
            var options = sp.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;

            return new MemoryBuilder()
                .WithAzureOpenAITextEmbeddingGenerationService(options.EmbeddingsModelDeploymentName, options.Endpoint.ToString(), options.Key)
                .WithMemoryStore(sp.GetRequiredService<IMemoryStore>())
                .Build();
        });
    }
}
