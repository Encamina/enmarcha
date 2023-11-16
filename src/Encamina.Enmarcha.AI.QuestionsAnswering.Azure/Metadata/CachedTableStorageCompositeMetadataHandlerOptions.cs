using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure.Metadata;

/// <summary>
/// Configuration options for the <see cref="CachedTableStorageCompositeMetadataHandler"/>.
/// </summary>
public class CachedTableStorageCompositeMetadataHandlerOptions
{
    /// <summary>
    /// Gets or sets the table connection string.
    /// </summary>
    public string TableConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the table name. By default it is '<c>Metadata</c>'.
    /// </summary>
    public string TableName { get; set; } = @"Metadata";

    /// <summary>
    /// Gets or sets the absolute expiration time, relative to now in seconds for cache to store values
    /// retrieved from the Table Storage, to improve performance. Defaults to <c>86400</c> (i.e., 24 hours - 1 day).
    /// </summary>
    public double CacheAbsoluteExpirationSeconds { get; set; } = 86400d;

    /// <summary>
    /// Gets or sets the execution order for the handler. Defaults to '<c>10</c>'.
    /// </summary>
    public int Order { get; set; } = 10;

    /// <summary>
    /// Gets or sets the logical operation to set when handling the metadata options.
    /// </summary>
    public LogicalOperation MetadataLogicalOperation { get; set; } = LogicalOperation.Or;
}
