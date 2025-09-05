using Azure;
using Azure.Core;
using Azure.Search.Documents.Indexes;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.Data.AzureAISearch;
using Encamina.Enmarcha.Data.Qdrant.Abstractions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Memory;

using Qdrant.Client;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure memory connectors for Semantic Kernel.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/memories/"/>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Azure AI Search as a vector database for a vector store (<see cref="VectorStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension method requires <see cref="AzureAISearchOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tokenCredential">
    /// The <see cref="TokenCredential"/> used to authenticate with Azure AI Search when
    /// <see cref="AzureAISearchOptions.UseTokenCredentialAuthentication"/> is <c>true</c>.
    /// When <see cref="AzureAISearchOptions.UseTokenCredentialAuthentication"/> is <c>false</c>, this parameter is ignored and <see cref="AzureAISearchOptions.Key"/> is used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureAISearchMemoryStore(this IServiceCollection services, TokenCredential? tokenCredential = null)
        => services.AddSingleton<VectorStore>(sp => CreateAzureAISearchVectorStore(sp, tokenCredential));

    /// <summary>
    /// Adds Qdrant as a vector database for a vector store (<see cref="VectorStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension method requires <see cref="QdrantOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQdrantMemoryStore(this IServiceCollection services)
        => services.AddSingleton<VectorStore>(CreateQdrantVectorStore);

    /// <summary>
    /// Adds Azure AI Search as a named vector database for a vector store (<see cref="VectorStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension method requires <see cref="AzureAISearchOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="memoryProviderName">The name/key to identify the memory provider registration.</param>
    /// <param name="tokenCredential">
    /// The <see cref="TokenCredential"/> used to authenticate with Azure AI Search when
    /// <see cref="AzureAISearchOptions.UseTokenCredentialAuthentication"/> is <c>true</c>.
    /// When <see cref="AzureAISearchOptions.UseTokenCredentialAuthentication"/> is <c>false</c>, this parameter is ignored and <see cref="AzureAISearchOptions.Key"/> is used.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureAISearchNamedMemoryStore(this IServiceCollection services, string memoryProviderName, TokenCredential? tokenCredential = null)
    {
        Guard.IsNotNullOrEmpty(memoryProviderName);
        return services.AddKeyedSingleton<VectorStore>(memoryProviderName, (sp, _) => CreateAzureAISearchVectorStore(sp, tokenCredential));
    }

    /// <summary>
    /// Adds Qdrant as a named vector database for a vector store (<see cref="VectorStore"/>) to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <remarks>
    /// This extension method requires <see cref="QdrantOptions"/> to be already configured.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="memoryProviderName">The name/key to identify the memory provider registration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQdrantNamedMemoryStore(this IServiceCollection services, string memoryProviderName)
    {
        Guard.IsNotNullOrEmpty(memoryProviderName);
        return services.AddKeyedSingleton<VectorStore>(memoryProviderName, (sp, _) => CreateQdrantVectorStore(sp));
    }

    private static AzureAISearchVectorStore CreateAzureAISearchVectorStore(IServiceProvider sp, TokenCredential? tokenCredential)
    {
        var opts = sp.GetRequiredService<IOptions<AzureAISearchOptions>>().Value
                   ?? throw new InvalidOperationException("AzureAISearchOptions not configured.");

        var indexClient = opts.UseTokenCredentialAuthentication
                        ? new SearchIndexClient(opts.Endpoint, tokenCredential!)
                        : new SearchIndexClient(opts.Endpoint, new AzureKeyCredential(opts.Key!));

        return new AzureAISearchVectorStore(indexClient);
    }

    private static QdrantVectorStore CreateQdrantVectorStore(IServiceProvider sp)
    {
        var opts = sp.GetRequiredService<IOptions<QdrantOptions>>().Value
                   ?? throw new InvalidOperationException("QdrantOptions not configured.");

        var loggerFactory = sp.GetService<ILoggerFactory>();

        var qdrantClient = new QdrantClient(address: opts.Host, apiKey: opts.ApiKey, loggerFactory: loggerFactory);

        // ownsClient: true => disposing the vector store will also dispose the QdrantClient.
        return new QdrantVectorStore(qdrantClient, ownsClient: true);
    }
}