using System.Text.RegularExpressions;

using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.TextSplitters;

/// <summary>
/// The recommended implementation of <see cref="IEnrichedTextSplitter"/> for generic texts with metadata support.
/// It splits texts in order until the chunks are small enough. It will try to keep all paragraphs
/// (and then sentences, and then words) together as long as possible.
/// </summary>
public class EnrichedRecursiveCharacterTextSplitter : EnrichedTextSplitter
{
    private static readonly string[] HeaderLevels = ["##", "###", "####", "#####", "######"];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrichedRecursiveCharacterTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the enriched recursive character text splitter.</param>
    public EnrichedRecursiveCharacterTextSplitter(IOptionsMonitor<EnrichedTextSplitterOptions> options) : base(options)
    {
    }

    /// <inheritdoc/>
    public override IEnumerable<(IDictionary<string, string> Metadata, string Text)> Split(string text, Func<string, int> lengthFunction, EnrichedTextSplitterOptions options)
    {
        var chunks = new List<(IDictionary<string, string> Metadata, string Text)>();

        if (string.IsNullOrWhiteSpace(text))
        {
            return chunks;
        }

        var context = new Dictionary<string, object>();
        string? separator = null;

        // Find appropriate separator (H1 first, then other headers, then default separators)
        foreach (var s in HeaderLevels)
        {
            if (s == string.Empty || text.Contains(s, StringComparison.Ordinal))
            {
                separator = s;
                break;
            }
        }

        var splits = (separator != null ? text.Split(separator, StringSplitOptions.RemoveEmptyEntries) : [text]).Select(s => s.Trim());

        var goodSplits = new List<string>();

        foreach (var split in splits)
        {
            if (lengthFunction(split) < options.ChunkSize)
            {
                goodSplits.Add(split);
            }
            else
            {
                if (goodSplits.Count > 0)
                {
                    var mergedChunks = MergeSplits(goodSplits, separator, lengthFunction, options);
                    foreach (var chunk in mergedChunks)
                    {
                        var metadata = ExtractAndUpdateMetadata(chunk, ref context);
                        chunks.Add((metadata, chunk.Trim()));
                    }

                    goodSplits.Clear();
                }

                var otherChunks = Split(split, lengthFunction, options);
                chunks.AddRange(otherChunks);
            }
        }

        if (goodSplits.Count > 0)
        {
            var mergedChunks = MergeSplits(goodSplits, separator, lengthFunction, options);
            foreach (var chunk in mergedChunks)
            {
                var metadata = ExtractAndUpdateMetadata(chunk, ref context);
                chunks.Add((metadata, chunk.Trim()));
            }
        }

        return chunks.Where(chunk => !string.IsNullOrWhiteSpace(chunk.Text));
    }

    /// <summary>
    /// Extracts metadata from the chunk and updates the context.
    /// </summary>
    /// <param name="chunk">The text chunk to extract metadata from.</param>
    /// <param name="context">The current context dictionary to update.</param>
    /// <returns>A dictionary containing the metadata for this chunk.</returns>
    private static Dictionary<string, string> ExtractAndUpdateMetadata(string chunk, ref Dictionary<string, object> context)
    {
        var currentMeta = ExtractMetadata(chunk);
        context = UpdateContext(currentMeta, context);

        var metadata = new Dictionary<string, string>();
        foreach (var (key, value) in context)
        {
            metadata[key] = value is List<string> list ? string.Join(", ", list) : value.ToString() ?? string.Empty;
        }

        return metadata;
    }

    /// <summary>
    /// Extracts metadata from markdown text including headers (H1-H6) and bold text.
    /// </summary>
    /// <param name="text">The markdown text to extract metadata from.</param>
    /// <returns>A dictionary containing the extracted metadata.</returns>
    private static Dictionary<string, object> ExtractMetadata(string text)
    {
        var metadata = new Dictionary<string, object>();

        // Extract Header 1
        var header1 = Regex.Matches(text, @"^# (.+)", RegexOptions.Multiline);
        if (header1.Count > 0)
        {
            metadata["Header_1"] = header1[0].Groups[1].Value;
        }

        // Extract Headers 2-6
        for (var level = 2; level <= 6; level++)
        {
            var pattern = $@"^{new string('#', level)} (.+)";
            var headers = Regex.Matches(text, pattern, RegexOptions.Multiline);
            if (headers.Count > 0)
            {
                metadata[$"Header_{level}"] = headers.Select(m => m.Groups[1].Value).ToList();
            }
        }

        // Extract Bold text
        var bolds = Regex.Matches(text, @"\*\*(.+?)\*\*");
        if (bolds.Count > 0)
        {
            metadata["Bold"] = bolds.Select(m => m.Groups[1].Value).ToList();
        }

        return metadata;
    }

    /// <summary>
    /// Updates the context by merging current metadata with previous context,
    /// maintaining hierarchical structure.
    /// </summary>
    private static Dictionary<string, object> UpdateContext(Dictionary<string, object> currentMeta, Dictionary<string, object> previousContext)
    {
        var context = new Dictionary<string, object>(previousContext);
        var levels = new[] { "Header_1", "Header_2", "Header_3", "Header_4", "Header_5", "Header_6", "Bold" };

        for (var i = 0; i < levels.Length; i++)
        {
            var key = levels[i];

            if (currentMeta.ContainsKey(key))
            {
                context[key] = currentMeta[key];

                // Remove all lower level keys
                for (var j = i + 1; j < levels.Length; j++)
                {
                    context.Remove(levels[j]);
                }
            }
            else if (context.ContainsKey(key) && context[key] is List<string> list && list.Count > 0)
            {
                context[key] = new List<string> { list[^1] };
            }
        }

        return context;
    }
}
