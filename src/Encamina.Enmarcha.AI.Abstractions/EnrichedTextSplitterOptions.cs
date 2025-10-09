using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Options for enriched text splitters.
/// </summary>
public class EnrichedTextSplitterOptions : TextSplitterOptions
{
    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) in each chunk.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public new int ChunkSize { get; init; } = 1000;
}
