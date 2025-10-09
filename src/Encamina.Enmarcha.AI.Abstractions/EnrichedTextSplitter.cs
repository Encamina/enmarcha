using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base class for enriched text splitters.
/// </summary>
public abstract class EnrichedTextSplitter : TextSplitter, IEnrichedTextSplitter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnrichedTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the enriched text splitter.</param>
    protected EnrichedTextSplitter(IOptionsMonitor<EnrichedTextSplitterOptions> options) : base(options)
    {
        var opts = options.CurrentValue;

        ChunkSize = opts.ChunkSize;
    }

    /// <inheritdoc/>
    public new int ChunkSize { get; }

    /// <inheritdoc/>
    public abstract IEnumerable<(IDictionary<string, string> Metadata, string Text)> Split(string text, Func<string, int> lengthFunction, EnrichedTextSplitterOptions options);

    /// <inheritdoc/>
    public new virtual IEnumerable<(IDictionary<string, string> Metadata, string Text)> Split(string text, Func<string, int> lengthFunction)
    {
        return Split(text, lengthFunction, new EnrichedTextSplitterOptions()
        {
            ChunkSize = ChunkSize,
        });
    }

    /// <summary>
    /// Implements the base <see cref="TextSplitter"/> Split method by delegating to the enriched version and extracting only the text parts.
    /// </summary>
    /// <param name="text">The text to be split.</param>
    /// <param name="lengthFunction">Length function used to calculate the length of a string.</param>
    /// <param name="options">Custom options used for splitting.</param>
    /// <returns>An IEnumerable of smaller text chunks.</returns>
    public override IEnumerable<string> Split(string text, Func<string, int> lengthFunction, TextSplitterOptions options) => throw new NotImplementedException();
}
