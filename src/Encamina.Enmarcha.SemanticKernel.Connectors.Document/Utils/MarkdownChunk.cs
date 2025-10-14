using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// Represents a chunk of markdown content with associated metadata.
/// </summary>
public class MarkdownChunk
{
    /// <summary>
    /// Gets or sets the markdown content of the chunk.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the metadata associated with the chunk, including headers and formatting information.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the token count of the chunk content.
    /// </summary>
    [JsonIgnore]
    public int TokenCount { get; set; }
}

