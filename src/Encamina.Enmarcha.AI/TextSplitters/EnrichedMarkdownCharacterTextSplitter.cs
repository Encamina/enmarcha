using System.Text.RegularExpressions;

using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.TextSplitters;

/// <summary>
/// The recommended implementation of <see cref="IEnrichedTextSplitter"/> for generic texts with metadata support.
/// It splits texts in order until the chunks are small enough. It will try to keep all paragraphs
/// (and then sentences, and then words) together as long as possible.
/// </summary>
public class EnrichedMarkdownCharacterTextSplitter : EnrichedTextSplitter
{
    private static readonly string[] HeaderLevels = ["##", "###", "####", "#####", "######"];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrichedMarkdownCharacterTextSplitter"/> class.
    /// </summary>
    /// <param name="options">The options to use when configuring the enriched recursive character text splitter.</param>
    public EnrichedMarkdownCharacterTextSplitter(IOptionsMonitor<TextSplitterOptions> options) : base(options)
    {
    }

    /// <inheritdoc/>
    public override IEnumerable<(IDictionary<string, string> Metadata, string Text)> SplitWithMetadata(string text, Func<string, int> lengthFunction, TextSplitterOptions options)
    {
        var h1Sections = SplitByH1(text);
        var context = new Dictionary<string, string>();

        foreach (var h1Section in h1Sections)
        {
            var chunks = lengthFunction(h1Section) <= options.ChunkSize ? [h1Section] : RecursiveSplit(h1Section, options.ChunkSize, lengthFunction, [.. HeaderLevels]);

            foreach (var chunk in chunks)
            {
                // Skip chunks that are too small (less than 30 tokens)
                if (lengthFunction(chunk) < 30)
                {
                    continue;
                }

                var currentMetadata = ExtractMetadata(chunk);
                context = UpdateContext(currentMetadata, context);

                yield return (new Dictionary<string, string>(context), chunk.Trim());
            }
        }
    }

    /// <summary>
    /// Splits text by H1 headers, keeping each H1 section together.
    /// </summary>
    private static List<string> SplitByH1(string text)
    {
        var matches = Regex.Matches(text, @"^# .+", RegexOptions.Multiline);
        var sections = new List<string>();

        foreach (var (match, index) in matches.Select((m, i) => (m, i)))
        {
            var start = match.Index;
            var end = index + 1 < matches.Count ? matches[index + 1].Index : text.Length;
            var section = text[start..end].Trim();

            sections.Add(section);
        }

        // If no H1 found, return the entire text
        if (sections.Count == 0)
        {
            sections.Add(text);
        }

        return sections;
    }

    /// <summary>
    /// Recursively splits text by headers and delimiters.
    /// </summary>
    private static List<string> RecursiveSplit(string text, int maxTokens, Func<string, int> lengthFunction, List<string> headerLevels)
    {
        if (lengthFunction(text) <= maxTokens)
        {
            return [text];
        }

        if (headerLevels.Count == 0)
        {
            return SplitByDelimiters(text, maxTokens, lengthFunction);
        }

        var header = headerLevels[0];
        var pattern = $@"(?=^{Regex.Escape(header)} )";
        var sections = Regex.Split(text, pattern, RegexOptions.Multiline);

        if (sections.Length == 1)
        {
            return RecursiveSplit(text, maxTokens, lengthFunction, headerLevels.Skip(1).ToList());
        }

        var chunks = new List<string>();
        var current = string.Empty;

        foreach (var section in sections)
        {
            var trimmedSection = section.Trim();
            if (string.IsNullOrWhiteSpace(trimmedSection))
            {
                continue;
            }

            var tentative = string.IsNullOrEmpty(current) ? trimmedSection : current + "\n" + trimmedSection;

            if (lengthFunction(tentative) <= maxTokens)
            {
                current = tentative;
            }
            else
            {
                if (!string.IsNullOrEmpty(current))
                {
                    chunks.Add(current.Trim());
                }

                if (lengthFunction(trimmedSection) > maxTokens)
                {
                    var subChunks = RecursiveSplit(trimmedSection, maxTokens, lengthFunction, [.. headerLevels.Skip(1)]);

                    chunks.AddRange(subChunks);
                    current = string.Empty;
                }
                else
                {
                    current = trimmedSection;
                }
            }
        }

        if (!string.IsNullOrEmpty(current))
        {
            chunks.Add(current.Trim());
        }

        return chunks;
    }

    /// <summary>
    /// Splits text by delimiters when header splitting is not enough.
    /// </summary>
    private static List<string> SplitByDelimiters(string text, int maxTokens, Func<string, int> lengthFunction)
    {
        var delimiters = new[] { @"\n\s*\n", @"\n", @"\.", @";", @"," };

        foreach (var delim in delimiters)
        {
            var parts = Regex.Split(text, delim);
            var chunks = new List<string>();
            var current = string.Empty;

            foreach (var part in parts)
            {
                var tentative = current + part + "\n";

                if (lengthFunction(tentative) <= maxTokens)
                {
                    current = tentative;
                }
                else
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        chunks.Add(current.Trim());
                    }

                    current = part + "\n";
                }
            }

            if (!string.IsNullOrEmpty(current))
            {
                chunks.Add(current.Trim());
            }

            // Check if all chunks are within the limit
            if (chunks.All(c => lengthFunction(c) <= maxTokens))
            {
                return chunks;
            }
        }

        return [text];
    }

    /// <summary>
    /// Extracts metadata (headers and bold text) from a chunk.
    /// </summary>
    private static Dictionary<string, string> ExtractMetadata(string text)
    {
        var metadata = new Dictionary<string, string>();

        // Extract H1
        var header1Match = Regex.Match(text, @"^# (.+)", RegexOptions.Multiline);
        if (header1Match.Success)
        {
            metadata["Header_1"] = header1Match.Groups[1].Value;
        }

        // Extract H2-H6
        foreach (var level in Enumerable.Range(2, 5))
        {
            var pattern = $@"^{new string('#', level)} (.+)";
            var matches = Regex.Matches(text, pattern, RegexOptions.Multiline);

            if (matches.Count > 0)
            {
                var headers = string.Join(", ", matches.Select(m => m.Groups[1].Value));
                metadata[$"Header_{level}"] = headers;
            }
        }

        // Extract bold text
        var boldMatches = Regex.Matches(text, @"\*\*(.+?)\*\*");
        if (boldMatches.Count > 0)
        {
            var bolds = string.Join(", ", boldMatches.Select(m => m.Groups[1].Value));
            metadata["Bold"] = bolds;
        }

        return metadata;
    }

    /// <summary>
    /// Updates the context with new metadata, maintaining hierarchical structure.
    /// </summary>
    private static Dictionary<string, string> UpdateContext(Dictionary<string, string> currentMetadata, Dictionary<string, string> previousContext)
    {
        var context = new Dictionary<string, string>(previousContext);
        var levels = new[] { "Header_1", "Header_2", "Header_3", "Header_4", "Header_5", "Header_6", "Bold" };

        foreach (var (key, index) in levels.Select((level, idx) => (level, idx)))
        {
            if (currentMetadata.ContainsKey(key))
            {
                context[key] = currentMetadata[key];

                // Remove lower level headers
                foreach (var lowerLevelKey in levels.Skip(index + 1))
                {
                    context.Remove(lowerLevelKey);
                }
            }
            else if (context.ContainsKey(key))
            {
                // Keep only the last header if it's a list
                var value = context[key];
                if (value.Contains(','))
                {
                    var parts = value.Split(',');
                    context[key] = parts[^1].Trim();
                }
            }
        }

        return context;
    }
}
