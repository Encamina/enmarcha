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
    protected EnrichedTextSplitter(IOptionsMonitor<TextSplitterOptions> options) : base(options)
    {
    }

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

    /// <inheritdoc/>
    public override IEnumerable<string> Split(string text, Func<string, int> lengthFunction, TextSplitterOptions options)
    {
        return SplitWithMetadata(text, lengthFunction, options).Select(x => x.Text);
    }
}
