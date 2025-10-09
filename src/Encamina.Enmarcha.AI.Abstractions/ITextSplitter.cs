namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents a text splitter, which splits a text into chunks of text.
/// </summary>
public interface ITextSplitter
{
    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) overlapping between chunks.
    /// </summary>
    int ChunkOverlap { get; }

    /// <summary>
    /// Gets the number of elements (characters, tokens, etc.) in each chunk.
    /// </summary>
    [Obsolete("Use IEnrichedTextSplitter.ChunkSize for enriched metadata support.")]
    int ChunkSize { get; }

    /// <summary>
    /// Gets the collection of separator characters to use when splitting the text and creating chunks.
    /// </summary>
    IList<string> Separators { get; }

    /// <summary>
    /// Merges splits into chunks of text, using the specified separator, length function and <see cref="TextSplitterOptions"/>.
    /// </summary>
    /// <param name="splits">The collection of splits to merge into chunks.</param>
    /// <param name="separator">The separator to use between splits.</param>
    /// <param name="lengthFunction">The function to use to calculate the length (or size) of each chunk, as specified by <paramref name="options"/> <see cref="TextSplitterOptions.ChunkSize"/>.</param>
    /// <param name="options">Custom options used for merge.</param>
    /// <returns>A collection of chunks built from the splits.</returns>
    IEnumerable<string> MergeSplits(IEnumerable<string> splits, string separator, Func<string, int> lengthFunction, TextSplitterOptions options);

    /// <summary>
    /// Merges splits into chunks of text, using the specified separator and length function.
    /// </summary>
    /// <param name="splits">The collection of splits to merge into chunks.</param>
    /// <param name="separator">The separator to use between splits.</param>
    /// <param name="lengthFunction">The function to use to calculate the length (or size) of each chunk, as specified by <see cref="ChunkSize"/>.</param>
    /// <returns>A collection of chunks built from the splits.</returns>
    IEnumerable<string> MergeSplits(IEnumerable<string> splits, string separator, Func<string, int> lengthFunction);

    /// <summary>
    /// Joins chunks into a single string using the specified separator.
    /// </summary>
    /// <param name="chunks">The collection of chunks to join.</param>
    /// <param name="separator">The separator to use between chunks.</param>
    /// <returns>A single string with all the chunks joined together by the specified separator.</returns>
    string JoinChunks(IEnumerable<string> chunks, string separator);

    /// <summary>
    /// Splits the specified text, using the specified length function.
    /// </summary>
    /// <param name="text">The text to split.</param>
    /// <param name="lengthFunction">A function to use to calculate the length (or size) of each split, usually specified by <see cref="ChunkSize"/>.</param>
    /// <returns>A collection of text splits.</returns>
    [Obsolete("Use IEnrichedTextSplitter.Split for enriched metadata support.")]
    IEnumerable<string> Split(string text, Func<string, int> lengthFunction);

    /// <summary>
    /// Splits the specified text, using the specified length function and specified <see cref="TextSplitterOptions"/>.
    /// </summary>
    /// <param name="text">The text to be split.</param>
    /// <param name="lengthFunction">Length function used to calculate the length of a string.</param>
    /// <param name="options">Custom options used for splitting.</param>
    /// <returns>An IEnumerable of smaller text chunks.</returns>
    [Obsolete("Use IEnrichedTextSplitter.Split for enriched metadata support.")]
    IEnumerable<string> Split(string text, Func<string, int> lengthFunction, TextSplitterOptions options);
}
