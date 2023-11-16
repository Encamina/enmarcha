using System.Net.Security;

using Encamina.Enmarcha.Core;
using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Data.Cosmos.Resources;

using Microsoft.Azure.Cosmos;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Initializer for Cosmos DB connections that provides access to containers and handles the client's lifecycle.
/// </summary>
/// <remarks>
/// cosa <see href="https://devblogs.microsoft.com/cosmosdb/improve-net-sdk-initialization/"/>.
/// </remarks>
internal sealed class CosmosInitializer : ICosmosInitializer
{
    private readonly CosmosOptions options;

    private readonly CosmosClient client;

    private readonly object disposedLock = new();

    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosInitializer"/> class.
    /// </summary>
    /// <param name="options">Configuration options for Cosmos DB connection.</param>
    public CosmosInitializer(IOptions<CosmosOptions> options)
    {
        this.options = options.Value;
        client = new CosmosClient(this.options.Endpoint, this.options.AuthKey, BuildCosmosClientOptions(this.options));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public Container GetContainer(string containerName)
    {
        return string.IsNullOrWhiteSpace(options.Database)
            ? throw new MissingConfigurationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.MissingDatabaseConfigurationParameterException)))
            : client.GetContainer(options.Database, containerName);
    }

    /// <inheritdoc/>
    public Container GetContainer(string database, string containerName)
    {
        return client.GetContainer(database, containerName);
    }

    private static CosmosClientOptions BuildCosmosClientOptions(CosmosOptions options)
    {
        var cosmosClientOptions = new CosmosClientOptions()
        {
            ConnectionMode = options.ConnectionMode,
            MaxRetryAttemptsOnRateLimitedRequests = options.MaxRetriesOnThrottling,
            MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(options.MaxRetryWaitTimeInSeconds),
            AllowBulkExecution = options.AllowBulkExecution,
        };

        // See the following information to understand why the `HttpClientFactory` is configure in this way:
        // https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21#disable-ssl-validation
        if (options.UseWithCosmosDbEmulator)
        {
            cosmosClientOptions.ConnectionMode = ConnectionMode.Gateway;
            cosmosClientOptions.HttpClientFactory = () =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) =>
                    {
                        return errors == SslPolicyErrors.None && HttpClientHandler.DangerousAcceptAnyServerCertificateValidator(sender, certificate, chain, errors);
                    },
                };

                return new HttpClient(httpMessageHandler);
            };
        }

        return cosmosClientOptions;
    }

    private void Dispose(bool disposing)
    {
        lock (disposedLock)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                client.Dispose();
            }

            isDisposed = true;
        }
    }
}
