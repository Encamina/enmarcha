using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.TextSplitters;

/// <summary>
/// The recomended implementation of <see cref="ITextSplitter"/> for generic texts. It splits texts in order until the chunks are small
/// enough (based on <see cref="ITextSplitter.ChunkSize"/>. It will try to keep all paragraphs (and then sentences, and then words) together
/// as long as possible, since those would generically seem to be the strongest semantically related pieces of text that could be splitted.
/// </summary>
public class RecursiveCharacterTextSplitter : TextSplitter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecursiveCharacterTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the recursive character text splitter.</param>
    public RecursiveCharacterTextSplitter(IOptionsMonitor<TextSplitterOptions> options) : base(options)
    {
    }

    /// <inheritdoc/>
    public override IEnumerable<string> Split(string text, Func<string, int> lengthFunction)
    {
        var chunks = new List<string>();

        string separator = null;

        foreach (var s in Separators)
        {
            if (s == string.Empty || text.Contains(s, StringComparison.OrdinalIgnoreCase))
            {
                separator = s;
                break;
            }
        }

        var splits = (separator != null ? text.Split(separator, StringSplitOptions.RemoveEmptyEntries) : new[] { text }).Select(s => s.Trim());

        var goodSplits = new List<string>();

        foreach (var split in splits)
        {
            if (lengthFunction(split) < ChunkSize)
            {
                goodSplits.Add(split);
            }
            else
            {
                if (goodSplits.Any())
                {
                    chunks.AddRange(MergeSplits(goodSplits, separator, lengthFunction));
                    goodSplits = new List<string>();
                }

                var otherChunks = Split(split, lengthFunction);
                chunks.AddRange(otherChunks);
            }
        }

        if (goodSplits.Any())
        {
            chunks.AddRange(MergeSplits(goodSplits, separator, lengthFunction));
        }

        return chunks;
    }
}

