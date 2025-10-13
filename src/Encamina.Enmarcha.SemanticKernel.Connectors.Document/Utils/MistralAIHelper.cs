using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.OpenAI.Abstractions;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// A helper class for handling operations related to MistralAI.
/// </summary>
internal static class MistralAIHelper
{
    /// <summary>
    /// Splits a PDF stream into multiple parts, each containing a specified number of pages.
    /// </summary>
    /// <param name="stream">The input PDF stream.</param>
    /// <param name="pagesPerPart">The number of pages per split part.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of memory streams, each representing a part of the split PDF.</returns>
    public static async Task<IReadOnlyList<MemoryStream>> SplitPdfByPagesAsync(Stream stream, int pagesPerPart = 30, CancellationToken cancellationToken = default)
    {
        Guard.IsNotNull(stream);
        Guard.IsGreaterThan(pagesPerPart, 0);

        List<MemoryStream> resultStreams = [];

        try
        {
            // Reset stream position if seekable
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            // Read PDF bytes efficiently
            byte[] pdfBytes;
            if (stream is MemoryStream ms)
            {
                pdfBytes = ms.ToArray();
            }
            else
            {
                using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer, cancellationToken);
                pdfBytes = buffer.ToArray();
            }

            using var sourcePdf = PdfDocument.Open(pdfBytes);
            var totalPages = sourcePdf.NumberOfPages;
            var estimatedParts = (int)Math.Ceiling((double)totalPages / pagesPerPart);

            resultStreams.Capacity = estimatedParts;

            // Split PDF into parts
            for (var startPage = 1; startPage <= totalPages; startPage += pagesPerPart)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var endPage = Math.Min(startPage + pagesPerPart - 1, totalPages);
                var builder = new PdfDocumentBuilder();

                for (var pageNumber = startPage; pageNumber <= endPage; pageNumber++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    builder.AddPage(sourcePdf, pageNumber);
                }

                var partBytes = builder.Build();
                var partStream = new MemoryStream(partBytes.Length);

                await partStream.WriteAsync(partBytes, cancellationToken);
                partStream.Position = 0;

                resultStreams.Add(partStream);
            }

            return resultStreams.AsReadOnly();
        }
        catch
        {
            // Clean up all created streams on error
            foreach (var partStream in resultStreams.Where(partStream => partStream is not null))
            {
                await partStream.DisposeAsync();
            }

            resultStreams.Clear();
            throw;
        }
    }

    /// <summary>
    /// Builds a PDF data URL from a stream.
    /// </summary>
    /// <param name="stream">The PDF stream to convert.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the PDF data URL.</returns>
    public static async Task<string> BuildPdfDataUrlAsync(Stream stream, CancellationToken ct)
    {
        // Avoid re-reading if the stream is already in memory
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        // If it's heavy, use ArrayPool to avoid putting pressure on the GC
        using var ms = new MemoryStream(capacity: stream.CanSeek ? (int)Math.Min(stream.Length, int.MaxValue) : 0);
        await stream.CopyToAsync(ms, ct).ConfigureAwait(false);

        var bytes = ms.TryGetBuffer(out var seg) ? [.. seg] : ms.ToArray();

        var base64 = Convert.ToBase64String(bytes);

        return $"data:application/pdf;base64,{base64}";
    }

    /// <summary>
    /// Extracts and combines markdown content from JSON pages with page separators.
    /// </summary>
    /// <param name="jsonContent">JSON string containing the pages array.</param>
    /// <returns>Combined markdown string with page separators.</returns>
    /// <exception cref="InvalidOperationException">Thrown when JSON structure is invalid.</exception>
    public static string ExtractAndCombineMarkdown(string jsonContent)
    {
        var pages = JsonNode.Parse(jsonContent)?["pages"]?.AsArray() ?? throw new InvalidOperationException("Invalid JSON structure: 'pages' array not found.");

        var markdownPages = pages.Select(page => page?["markdown"]?.GetValue<string>())
                                 .Where(markdown => !string.IsNullOrWhiteSpace(markdown));

        return string.Join(string.Empty, markdownPages);
    }

    /// <summary>
    /// Splits a Markdown document into chunks respecting its hierarchical structure.
    /// </summary>
    /// <param name="markdown">The Markdown text to split.</param>
    /// <param name="maxChunkSize">Maximum number of characters allowed per chunk.</param>
    /// <returns>An enumerable of Markdown chunks.</returns>
    /// 
    public class MarkdownChunk
    {
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public static List<MarkdownChunk> SplitMarkdown(string markdown, int maxTokens, Func<string, int> countTokens)
    {
        List<string> SplitByH1(string text)
        {
            var matches = Regex.Matches(text, @"^# .+", RegexOptions.Multiline);
            var sections = new List<string>();
            for (int i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index;
                var end = (i + 1 < matches.Count) ? matches[i + 1].Index : text.Length;
                var section = text.Substring(start, end - start).Trim();
                sections.Add(section);
            }
            return sections.Count > 0 ? sections : new List<string> { text };
        }

        List<string> SplitByDelimiters(string text, Func<string, int> tokenCounter, int maxToks)
        {
            var delimiters = new[] { @"\n\s*\n", @"\n", @"\.", @";", @"," };
            foreach (var delim in delimiters)
            {
                var parts = Regex.Split(text, delim);
                var chunks = new List<string>();
                var current = "";

                foreach (var part in parts)
                {
                    var tentative = string.IsNullOrEmpty(current) ? part : current + part + "\n";
                    if (tokenCounter(tentative) <= maxToks)
                    {
                        current = tentative;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(current))
                            chunks.Add(current.Trim());
                        current = part + "\n";
                    }
                }

                if (!string.IsNullOrWhiteSpace(current))
                    chunks.Add(current.Trim());

                if (chunks.All(c => tokenCounter(c) <= maxToks))
                    return chunks;
            }

            return new List<string> { text };
        }

        List<string> RecursiveSplit(string text, Func<string, int> tokenCounter, int maxToks, string[] headers)
        {
            if (tokenCounter(text) <= maxToks)
                return new List<string> { text };

            if (headers.Length == 0)
                return SplitByDelimiters(text, tokenCounter, maxToks);

            var header = headers[0];
            var pattern = $@"(?=^{Regex.Escape(header)} )";
            var sections = Regex.Split(text, pattern, RegexOptions.Multiline)
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .ToList();

            if (sections.Count == 1)
                return RecursiveSplit(text, tokenCounter, maxToks, headers.Skip(1).ToArray());

            var chunks = new List<string>();
            var current = "";

            foreach (var section in sections)
            {
                var tentative = string.IsNullOrEmpty(current) ? section : current + "\n" + section;
                if (tokenCounter(tentative) <= maxToks)
                {
                    current = tentative;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(current))
                        chunks.Add(current.Trim());

                    if (tokenCounter(section) > maxToks)
                    {
                        var subs = RecursiveSplit(section, tokenCounter, maxToks, headers.Skip(1).ToArray());
                        chunks.AddRange(subs);
                        current = "";
                    }
                    else
                    {
                        current = section;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(current))
                chunks.Add(current.Trim());

            return chunks;
        }

        Dictionary<string, object> ExtractMetadata(string text)
        {
            var metadata = new Dictionary<string, object>();

            var h1 = Regex.Match(text, @"^# (.+)", RegexOptions.Multiline);
            if (h1.Success)
                metadata["Header_1"] = h1.Groups[1].Value;

            for (int level = 2; level <= 6; level++)
            {
                var headerMatches = Regex.Matches(text, $"^{"#".PadLeft(level, '#')} (.+)", RegexOptions.Multiline);
                if (headerMatches.Count > 0)
                {
                    var headers = headerMatches.Select(m => m.Groups[1].Value).ToList();
                    metadata[$"Header_{level}"] = headers;
                }
            }

            var bolds = Regex.Matches(text, @"\*\*(.+?)\*\*");
            if (bolds.Count > 0)
            {
                metadata["Bold"] = bolds.Select(m => m.Groups[1].Value).ToList();
            }

            return metadata;
        }

        Dictionary<string, object> UpdateContext(Dictionary<string, object> currentMeta, Dictionary<string, object> previous)
        {
            var context = new Dictionary<string, object>(previous);
            var levels = Enumerable.Range(1, 6).Select(i => $"Header_{i}").Concat(new[] { "Bold" }).ToList();

            for (int i = 0; i < levels.Count; i++)
            {
                var key = levels[i];
                if (currentMeta.ContainsKey(key))
                {
                    context[key] = currentMeta[key];

                    foreach (var lowerKey in levels.Skip(i + 1))
                    {
                        context.Remove(lowerKey);
                    }
                }
                else if (context.TryGetValue(key, out var val) && val is List<string> list && list.Count > 0)
                {
                    context[key] = new List<string> { list.Last() };
                }
            }

            return context;
        }

        // MAIN
        var h1Sections = SplitByH1(markdown);
        var headerLevels = new[] { "##", "###", "####", "#####", "######" };
        var allChunks = new List<MarkdownChunk>();
        var context = new Dictionary<string, object>();

        foreach (var section in h1Sections)
        {
            var chunks = (countTokens(section) <= maxTokens)
                ? new List<string> { section }
                : RecursiveSplit(section, countTokens, maxTokens, headerLevels);

            foreach (var chunk in chunks)
            {
                if (countTokens(chunk) < 30)
                    continue;

                var currentMeta = ExtractMetadata(chunk);
                context = UpdateContext(currentMeta, context);

                allChunks.Add(new MarkdownChunk
                {
                    Content = chunk.Trim(),
                    Metadata = new Dictionary<string, object>(context)
                });
            }
        }

        return allChunks;
    }
}
