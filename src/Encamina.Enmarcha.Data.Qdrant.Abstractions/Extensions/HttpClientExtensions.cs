namespace Encamina.Enmarcha.Data.Qdrant.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpClient"/> when working with Qdrant.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Configures an <see cref="HttpClient"/> to work with Qdrant based on values from <see cref="QdrantOptions"/>.
    /// </summary>
    /// <param name="httpClient">The <see cref="HttpClient"/> to configure.</param>
    /// <param name="qdrantOptions">The <see cref="QdrantOptions"/> to use for configuration.</param>
    /// <returns>The <see cref="HttpClient"/> so that additional calls can be chained.</returns>
    public static HttpClient ConfigureHttpClientForQdrant(this HttpClient httpClient, QdrantOptions qdrantOptions)
    {
        httpClient.BaseAddress = qdrantOptions.BuildEndpoint();

        httpClient.DefaultRequestHeaders.Remove(Constants.QdrantApiKeyHeader);

        if (!string.IsNullOrWhiteSpace(qdrantOptions.ApiKey))
        {
            httpClient.DefaultRequestHeaders.Add(Constants.QdrantApiKeyHeader, qdrantOptions.ApiKey);
        }

        return httpClient;
    }
}
