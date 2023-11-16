using CommunityToolkit.Diagnostics;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base class for text splitters.
/// </summary>
public abstract class TextSplitter : ITextSplitter
{
    /// <summary>
    /// Default collection of separator characters to use when splitting the text and creating chunks.
    /// </summary>
    public static readonly string[] DefaultSeparators = { ".", "!", "?", ";", ":", "\r\n", "\n" };

    /// <summary>
    /// Initializes a new instance of the <see cref="TextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the text splitter.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the configured value for chunk overlap is greater than the configured value for chunk size.
    /// The value of chunks overlap must be smaller than the value of chunk size.
    /// </exception>
    protected TextSplitter(IOptionsMonitor<TextSplitterOptions> options)
    {
        var opts = options.CurrentValue;

        ChunkOverlap = opts.ChunkOverlap;
        ChunkSize = opts.ChunkSize;
        Separators = opts.Separators?.Any() == true ? opts.Separators : new List<string>(DefaultSeparators);

        if (ChunkOverlap > ChunkSize)
        {
            throw new InvalidOperationException(@"Configured value for chunk overlap is greater than configured value for chunk size. It must be smaller!");
        }
    }

    /// <inheritdoc/>
    public int ChunkOverlap { get; }

    /// <inheritdoc/>
    public int ChunkSize { get; }

    /// <inheritdoc/>
    public IList<string> Separators { get; }

    /// <inheritdoc/>
    public abstract IEnumerable<string> Split(string text, Func<string, int> lengthFunction);

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the parameters <paramref name="chunks"/> or <paramref name="separator"/> is <see langword="null"/>.
    /// </exception>"
    public virtual string JoinChunks(IEnumerable<string> chunks, string separator)
    {
        Guard.IsNotNull(chunks);
        Guard.IsNotNull(separator);

        var text = string.Join(separator, chunks).Trim();

        return string.IsNullOrWhiteSpace(text)
            ? null
            : text;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any of the parameters <paramref name="splits"/>, <paramref name="separator"/>, or <paramref name="lengthFunction"/> is <see langword="null"/>.
    /// </exception>"
    public virtual IEnumerable<string> MergeSplits(IEnumerable<string> splits, string separator, Func<string, int> lengthFunction)
    {
        Guard.IsNotNull(splits);
        Guard.IsNotNull(separator);
        Guard.IsNotNull(lengthFunction);

        string chunk = null;
        var chunks = new List<string>();
        var currentChunks = new Queue<string>();

        var total = 0;
        var separatorLength = lengthFunction(separator);

        foreach (var split in splits)
        {
            var splitLength = lengthFunction(split);
            var hasCurrentChunks = currentChunks.Any();

            if (hasCurrentChunks && total + splitLength + separatorLength > ChunkSize)
            {
                chunk = JoinChunks(currentChunks, separator);

                if (chunk != null)
                {
                    chunks.Add(chunk);
                }

                // Keep on dequeuing if:
                // - There are still chunks and their length is long
                // - There is a larger chunk than the chunk overlap
                while (
                    hasCurrentChunks
                    && (total > ChunkOverlap || (total + splitLength + separatorLength > ChunkSize && total > 0)))
                {
                    total -= lengthFunction(currentChunks.Dequeue()) + (currentChunks.Count > 1 ? separatorLength : 0);
                    hasCurrentChunks = currentChunks.Any();
                }
            }

            currentChunks.Enqueue(split);
            total += splitLength + (currentChunks.Count > 1 ? separatorLength : 0);
        }

        chunk = JoinChunks(currentChunks, separator);

        if (chunk != null)
        {
            chunks.Add(chunk);
        }

        return chunks;
    }
}
