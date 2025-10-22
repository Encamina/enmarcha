using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base class for enriched text splitters.
/// </summary>
public abstract class EnrichedTextSplitter : IEnrichedTextSplitter
{
    /// <summary>
    /// Default collection of separator characters to use when splitting the text and creating chunks.
    /// </summary>
    public static readonly string[] DefaultSeparators = { ".", "!", "?", ";", ":", "\r\n", "\n" };

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrichedTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the enriched text splitter.</param>
    protected EnrichedTextSplitter(IOptionsMonitor<TextSplitterOptions> options)
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
    public abstract IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)> SplitWithMetadata(string text, Func<string, int> lengthFunction, TextSplitterOptions options);

    /// <inheritdoc/>
    public virtual IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)> SplitWithMetadata(string text, Func<string, int> lengthFunction)
    {
        return SplitWithMetadata(text, lengthFunction, new TextSplitterOptions()
        {
            ChunkSize = ChunkSize,
        });
    }
}
