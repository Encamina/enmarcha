using Encamina.Enmarcha.Data.Qdrant;
using Encamina.Enmarcha.Data.Qdrant.Abstractions;
using Encamina.Enmarcha.Data.Qdrant.Abstractions.Extensions;

using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up a Qdrant database in a <see cref="IServiceCollection"/>.
/// </summary>
/// <seealso href="https://qdrant.tech/documentation/overview/"/>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds Qdrant snapshot handler to the <see cref="IServiceCollection"/> as a singleton service.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddQdrantSnapshotHandler(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(QdrantSnapshotHandler), (serviceProvider, httpClient) =>
        {
            var qdrantOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<QdrantOptions>>();
            var qdrantOptions = qdrantOptionsMonitor.CurrentValue;

            httpClient.ConfigureHttpClientForQdrant(qdrantOptions);

            qdrantOptionsMonitor.OnChange(newOptions =>
            {
                httpClient.ConfigureHttpClientForQdrant(qdrantOptions);
            });
        });

        return services.AddSingleton<IQdrantSnapshotHandler, QdrantSnapshotHandler>();
    }
}
