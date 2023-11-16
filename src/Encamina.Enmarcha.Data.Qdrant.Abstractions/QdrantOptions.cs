using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.Data.Qdrant.Abstractions;

/// <summary>
/// Configuration options for setting up a connection to Qdrant vector database.
/// </summary>
public sealed class QdrantOptions
{
    /// <summary>
    /// Gets the endpoint protocol and host (e.g. http://localhost).
    /// </summary>
    [Required]
    [Uri]
    public Uri Host { get; init; }

    /// <summary>
    /// Gets the endpoint port.
    /// </summary>
    [Range(0, 65535)]
    public int? Port { get; init; }

    /// <summary>
    /// Gets the vector size.
    /// </summary>
    /// <remarks>
    /// Defaults is <c>1536</c>.
    /// </remarks>
    [Range(1, int.MaxValue)]
    [Required]
    public int VectorSize { get; init; } = 1536;

    /// <summary>
    /// Gets an optional API Key used by Qdrant as a form of client authentication.
    /// </summary>
    [NotEmptyOrWhitespace]
    public string ApiKey { get; init; }

    /// <summary>
    /// Builds a Qdrant connection endpoint from the <see cref="Host"/> and <see cref="Port"/> properties.
    /// </summary>
    /// <returns>A valid Qdrant connection endpoint.</returns>
    public Uri BuildEndpoint()
    {
        UriBuilder builder = new(Host);

        if (Port.HasValue)
        {
            builder.Port = Port.Value;
        }

        return builder.Uri;
    }
}
