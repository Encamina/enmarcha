using System.Text.Json;

using Encamina.Enmarcha.Data.Qdrant.Abstractions;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Data.Qdrant;

/// <summary>
/// A handler for Qdrant snapshots.
/// </summary>
internal sealed class QdrantSnapshotHandler : IQdrantSnapshotHandler
{
    private const string CreationTime = @"creation_time";
    private const string Name = @"name";
    private const string OkStatus = @"ok";
    private const string Result = @"result";
    private const string Status = @"status";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantSnapshotHandler"/> class.
    /// </summary>
    /// <param name="httpClientFactory">
    /// An <see cref="IHttpClientFactory"/> used to create an HTTP client (<see cref="HttpClient"/>) for this API.
    /// Currently, Qdrant does not provides an SDK for most of its features, but it does provide a REST API.
    /// </param>
    /// <param name="logger">A logger.</param>
    public QdrantSnapshotHandler(IHttpClientFactory httpClientFactory, ILogger<IQdrantSnapshotHandler> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task CreateCollectionSnapshotAsync(string collectionName, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(QdrantSnapshotHandler));

        using var response = await httpClient.PostAsync($@"/collections/{collectionName}/snapshots", null, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var json = (await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken)).RootElement;
            var status = json.GetProperty(Status).GetString();

            if (OkStatus.Equals(status, StringComparison.OrdinalIgnoreCase))
            {
                var result = json.GetProperty(Result);
                var snapshotName = result.GetProperty(Name).GetString();
                var creationTime = result.GetProperty(CreationTime).GetString();

                logger.LogInformation($@"Successfully created snapshot for collection '{collectionName}' on '{creationTime ?? DateTime.UtcNow.ToString(@"o")}'. Snapshot name is '{snapshotName}'.");
            }
            else
            {
                logger.LogError($@"Failed creating snapshot for collection '{collectionName}'. Returned status was: {status}!");
            }
        }
        else
        {
            logger.LogError($@"Failed creating snapshot for collection '{collectionName}'. Response status was '{response.StatusCode}' and error was: {await response.Content.ReadAsStringAsync(cancellationToken)}.");
        }
    }
}
