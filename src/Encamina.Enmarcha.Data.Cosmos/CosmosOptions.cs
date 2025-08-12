using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

using Microsoft.Azure.Cosmos;

namespace Encamina.Enmarcha.Data.Cosmos;

/// <summary>
/// Configuration options for Cosmos DB connections.
/// </summary>
public class CosmosOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether bulk executions are allowed or not.
    /// </summary>
    public bool AllowBulkExecution { get; set; } = false;

    /// <summary>
    /// Gets or sets the authentication key required to connect with Azure Cosmos DB.
    /// </summary>
    [RequiredIf([nameof(UseDefaultAzureCredentialAuthentication), nameof(UseTokenCredentialAuthentication)], [false, false], failOnAnyCondition: false)]
    public string? AuthKey { get; set; }

    /// <summary>
    /// Gets or sets the connection mode. Defaults to <see cref="ConnectionMode.Direct"/>.
    /// </summary>
    public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.Direct;

    /// <summary>
    /// Gets or sets the database name to connect with Azure Cosmos DB.
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the Azure Cosmos DB service endpoint to use.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retries in the case where the request fails
    /// because the Azure Cosmos DB service has applied rate limiting on the client. Defaults to '<c>5</c>'.
    /// </summary>
    /// <value>
    /// The default value is 5. This means in the case where the request is rate limited,
    /// the same request will be issued for a maximum of 6 times to the server before
    /// an error is returned to the application.
    ///
    /// If the value of this property is set to 0, there will be no automatic retry on rate
    /// limiting requests from the client and the exception needs to be handled at the
    /// application level.
    /// </value>
    /// <remarks>
    /// <para>
    /// When a client is sending requests faster than the allowed rate, the service will return an HTTP Code 429 (Too Many Requests)
    /// to rate limit the client. The current implementation in the SDK will then wait for the amount of time the service tells it
    /// to wait and retry after the time has elapsed.
    /// </para>
    /// <para>
    /// For more information, see <see href="https://docs.microsoft.com/azure/cosmos-db/performance-tips#throughput">Handle rate limiting/request rate too large</see>.
    /// </para>
    /// </remarks>
    public int MaxRetriesOnThrottling { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum retry time in seconds for the Azure Cosmos DB service.
    /// </summary>
    /// <value>
    /// The default value is 5 seconds.
    /// </value>
    /// <remarks>
    /// <para>
    /// When a request fails due to a rate limiting error, the service sends back a response that contains a value indicating the client should not
    /// retry before the <see cref="Microsoft.Azure.Cosmos.CosmosException.RetryAfter"/> time period has elapsed.
    /// </para>
    /// <para>
    /// This property allows the application to set a maximum wait time for all retry attempts. If the cumulative wait time exceeds the this value, the
    /// client will stop retrying and return the error to the application.
    /// </para>
    /// <para>
    /// For more information, see <see href="https://docs.microsoft.com/azure/cosmos-db/performance-tips#throughput">Handle rate limiting/request rate too large</see>.
    /// </para>
    /// </remarks>
    public int MaxRetryWaitTimeInSeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether this configuration should use the Cosmos DB emulator for developer purposes <b>only</b>.
    /// Defaults to '<see langword="false"/>'.
    /// </summary>
    public bool UseWithCosmosDbEmulator { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the default Azure credential should be used for authentication. This is usually required when connecting with Managed Identities.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="false" />.
    /// </remarks>
    public bool UseDefaultAzureCredentialAuthentication { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether token credential authentication should be used.
    /// When set to <see langword="true"/>, a TokenCredential must be provided to the CosmosClient constructor at run-time.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="false" />.
    /// </remarks>
    public bool UseTokenCredentialAuthentication { get; set; } = false;
}
