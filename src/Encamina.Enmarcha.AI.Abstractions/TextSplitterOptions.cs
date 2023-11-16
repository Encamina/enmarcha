using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Optiones for text splitters.
/// </summary>
public class TextSplitterOptions
{
    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) overlapping between chunks.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int ChunkOverlap { get; init; }

    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) in each chunk.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int ChunkSize { get; init; }

    /// <summary>
    /// Gets the collection of separator characters to use when splitting the text and creating chunks.
    /// </summary>
    public IList<string> Separators { get; init; }
}
