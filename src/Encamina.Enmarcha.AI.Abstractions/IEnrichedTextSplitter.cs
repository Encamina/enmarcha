namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents a text splitter, which splits a text into chunks of text with metadata.
/// </summary>
public interface IEnrichedTextSplitter
{
    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) overlapping between chunks.
    /// </summary>
    int ChunkOverlap { get; }

    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) in each chunk.
    /// </summary>
    int ChunkSize { get; }

    /// <summary>
    /// Gets the collection of separator characters to use when splitting the text and creating chunks.
    /// </summary>
    IList<string> Separators { get; }

    /// <summary>
    /// Splits the specified text, using the specified length function.
    /// </summary>
    /// <param name="text">The text to split.</param>
    /// <param name="lengthFunction">A function to use to calculate the length (or size) of each split, usually specified by <see cref="ITextSplitter.ChunkSize"/>.</param>
    /// <returns>A collection of text splits with metadata. Each element contains a dictionary with metadata and the text chunk.</returns>
    IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)> SplitWithMetadata(string text, Func<string, int> lengthFunction);

    /// <summary>
    /// Splits the specified text, using the specified length function and specified <see cref="TextSplitterOptions"/>.
    /// </summary>
    /// <param name="text">The text to be split.</param>
    /// <param name="lengthFunction">Length function used to calculate the length of a string.</param>
    /// <param name="options">Custom options used for splitting.</param>
    /// <returns>A collection of text splits with metadata. Each element contains a dictionary with metadata and the text chunk.</returns>
    IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)> SplitWithMetadata(string text, Func<string, int> lengthFunction, TextSplitterOptions options);
}
