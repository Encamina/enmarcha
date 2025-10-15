using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using CommunityToolkit.Diagnostics;

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
    /// Extracts and combines markdown content from JSON pages, replacing base64 image references with simple filenames.
    /// </summary>
    /// <param name="jsonContent">JSON string containing the pages array with markdown and images.</param>
    /// <returns>Combined markdown string with image references replaced by filenames.</returns>
    /// <exception cref="InvalidOperationException">Thrown when JSON structure is invalid.</exception>
    public static string ExtractAndCombineMarkdown(string jsonContent)
    {
        Guard.IsNotNull(jsonContent);

        var pages = JsonNode.Parse(jsonContent)?["pages"]?.AsArray()
            ?? throw new InvalidOperationException("Invalid JSON structure: 'pages' array not found.");

        var fullContent = new StringBuilder();

        for (var pageIndex = 0; pageIndex < pages.Count; pageIndex++)
        {
            var page = pages[pageIndex];
            if (page is null)
            {
                continue;
            }

            // Extract image data from the page
            var imageData = ExtractImageDataFromPage(page);

            // Extract markdown content
            var pageMarkdown = page["markdown"]?.GetValue<string>()
                ?? page["content"]?.GetValue<string>()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(pageMarkdown))
            {
                continue;
            }

            // Replace image references with filenames
            var processedMarkdown = ReplaceImagesInMarkdown(pageMarkdown, imageData);
            fullContent.Append(processedMarkdown);
            fullContent.Append("\n\n");
        }

        return fullContent.ToString();
    }

    /// <summary>
    /// Replaces image references in the markdown with simple filenames instead of embedding base64 content.
    /// </summary>
    /// <param name="markdownStr">The markdown string containing image references.</param>
    /// <param name="imagesDict">Dictionary mapping image IDs to their base64 content.</param>
    /// <returns>The markdown string with replaced image references.</returns>
    private static string ReplaceImagesInMarkdown(string markdownStr, IDictionary<string, string> imagesDict)
    {
        if (imagesDict.Count == 0)
        {
            return markdownStr;
        }

        var result = markdownStr;

        foreach (var (imgName, _) in imagesDict)
        {
            var filename = $"{imgName}.png";
            result = result.Replace($"![{imgName}]({imgName})", $"![{filename}]({filename})");
        }

        return result;
    }

    /// <summary>
    /// Extracts image data from a JSON page node, handling multiple possible JSON structures.
    /// </summary>
    /// <param name="page">The JSON page node.</param>
    /// <returns>Dictionary mapping image IDs to their base64 content.</returns>
    private static Dictionary<string, string> ExtractImageDataFromPage(JsonNode page)
    {
        var imageData = new Dictionary<string, string>();
        var rawImages = page["images"] ?? page["image"];

        if (rawImages is null)
        {
            return imageData;
        }

        if (rawImages is JsonObject imagesObject)
        {
            var normalizedImages = new List<Dictionary<string, string>>();

            foreach (var (key, value) in imagesObject)
            {
                if (value is JsonObject imgObj)
                {
                    var imgId = imgObj["id"]?.GetValue<string>() ?? key;
                    var imgB64 = imgObj["image_base64"]?.GetValue<string>()
                        ?? imgObj["base64"]?.GetValue<string>()
                        ?? imgObj["data"]?.GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(imgB64))
                    {
                        imageData[imgId] = imgB64;
                    }
                }
                else if (value is JsonValue)
                {
                    // Assume value is the base64 string directly
                    var imgB64 = value.GetValue<string>();
                    if (!string.IsNullOrWhiteSpace(imgB64))
                    {
                        imageData[key] = imgB64;
                    }
                }
            }
        }
        else if (rawImages is JsonArray imagesArray)
        {
            for (var i = 0; i < imagesArray.Count; i++)
            {
                var img = imagesArray[i];
                if (img is not JsonObject imgObj)
                {
                    continue;
                }

                var imgId = imgObj["id"]?.GetValue<string>()
                    ?? imgObj["image_id"]?.GetValue<string>()
                    ?? $"image_{imageData.Count + 1}";

                var imgB64 = imgObj["image_base64"]?.GetValue<string>()
                    ?? imgObj["base64"]?.GetValue<string>()
                    ?? imgObj["data"]?.GetValue<string>();

                if (!string.IsNullOrWhiteSpace(imgB64))
                {
                    imageData[imgId] = imgB64;
                }
            }
        }

        return imageData;
    }

    /// <summary>
    /// Divide el markdown en trozos según el número máximo de caracteres para procesamiento LLM.
    /// </summary>
    /// <param name="markdown">El texto markdown a dividir.</param>
    /// <param name="maxChars">Número máximo de caracteres por trozo.</param>
    /// <returns>Lista de trozos de markdown.</returns>
    public static List<string> SplitMarkdownForRefinement(string markdown, int maxChars)
    {
        var parts = new List<string>();
        var current = new StringBuilder();
        var currentLength = 0;

        var lines = markdown.Split('\n');

        foreach (var line in lines)
        {
            var lineWithNewline = line + "\n";
            var lineLength = lineWithNewline.Length;

            if (currentLength + lineLength > maxChars && current.Length > 0)
            {
                parts.Add(current.ToString());
                current.Clear();
                current.Append(lineWithNewline);
                currentLength = lineLength;
            }
            else
            {
                current.Append(lineWithNewline);
                currentLength += lineLength;
            }
        }

        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }

        return parts;
    }

    /// <summary>
    /// Limpia el output del modelo LLM eliminando fences markdown y frases introductorias.
    /// </summary>
    /// <param name="text">El texto a limpiar.</param>
    /// <returns>Texto limpio sin artefactos del modelo.</returns>
    public static string CleanLLMOutput(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        text = text.Trim();

        // Quitar bloques ```markdown o similares al inicio
        if (text.StartsWith("```"))
        {
            var afterFirstFence = text.IndexOf('\n');
            if (afterFirstFence > 0)
            {
                text = text.Substring(afterFirstFence + 1);
            }
        }

        // Quitar bloques ``` al final
        if (text.EndsWith("```"))
        {
            var lastFenceIndex = text.LastIndexOf("```");
            if (lastFenceIndex > 0)
            {
                text = text.Substring(0, lastFenceIndex);
            }
        }

        // Eliminar frases introductorias comunes
        var bannedPhrases = new[]
        {
            "Aquí tienes", "texto refinado", "he mejorado",
            "refinado y corregido", "resultado final", "markdown final",
            "Here is", "here's the", "refined markdown", "cleaned version"
        };

        foreach (var phrase in bannedPhrases)
        {
            var index = text.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                // Buscar el final de la línea después de la frase
                var lineEndIndex = text.IndexOf('\n', index);
                if (lineEndIndex > 0)
                {
                    text = text.Substring(lineEndIndex + 1);
                }
            }
        }

        return text.Trim();
    }

    /// <summary>
    /// Splits a Markdown document into chunks respecting its hierarchical structure.
    /// </summary>
    /// <param name="markdown">The Markdown text to split.</param>
    /// <param name="maxTokens">Maximum number of tokens allowed per chunk.</param>
    /// <param name="countTokens">Token counter method.</param>
    /// <returns>An enumerable of Markdown chunks.</returns>
    public static List<MarkdownChunk> ChunkingMarkdown(string markdown, int maxTokens, Func<string, int> countTokens)
    {
        List<string> SplitByH1(string text)
        {
            var matches = Regex.Matches(text, @"^# .+", RegexOptions.Multiline);
            var sections = new List<string>();

            for (var i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index;
                var end = (i + 1 < matches.Count) ? matches[i + 1].Index : text.Length;
                var section = text.Substring(start, end - start).Trim();
                sections.Add(section);
            }

            return sections;
        }

        List<string> SplitByDelimiters(string text, int maxToks)
        {
            var delimiters = new[] { @"\n\s*\n", @"\n", @"\.", @";", @"," };

            foreach (var delim in delimiters)
            {
                var parts = Regex.Split(text, delim);
                var chunks = new List<string>();
                var current = "";

                foreach (var part in parts)
                {
                    if (countTokens(current + part) <= maxToks)
                    {
                        current += part + "\n";
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

                if (chunks.All(c => countTokens(c) <= maxToks))
                {
                    return chunks;
                }
            }

            return new List<string> { text };
        }

        List<string> RecursiveSplit(string text, int maxToks, string[] headers)
        {
            if (countTokens(text) <= maxToks)
            {
                return new List<string> { text };
            }

            if (headers.Length == 0)
            {
                return SplitByDelimiters(text, maxToks);
            }

            var header = headers[0];
            var pattern = $@"(?=^{Regex.Escape(header)} )";
            var sections = Regex.Split(text, pattern, RegexOptions.Multiline);

            if (sections.Length == 1)
            {
                return RecursiveSplit(text, maxToks, headers.Skip(1).ToArray());

            }

            var chunks = new List<string>();
            var current = "";

            foreach (var section in sections)
            {
                var trimmedSection = section.Trim();
                if (string.IsNullOrEmpty(trimmedSection))
                {
                    continue;
                }

                var tentative = string.IsNullOrEmpty(current) ? trimmedSection : current + "\n" + trimmedSection;

                if (countTokens(tentative) <= maxToks)
                {
                    current = tentative;
                }
                else
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        chunks.Add(current.Trim());
                    }

                    if (countTokens(trimmedSection) > maxToks)
                    {
                        var subs = RecursiveSplit(trimmedSection, maxToks, headers.Skip(1).ToArray());
                        chunks.AddRange(subs);
                        current = "";
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

        Dictionary<string, object> ExtractMetadata(string text)
        {
            var metadata = new Dictionary<string, object>();

            var h1 = Regex.Match(text, @"^# (.+)", RegexOptions.Multiline);
            if (h1.Success)
            {
                metadata["Header_1"] = h1.Groups[1].Value;
            }

            for (var level = 2; level <= 6; level++)
            {
                var pattern = $@"^{new string('#', level)} (.+)";
                var headerMatches = Regex.Matches(text, pattern, RegexOptions.Multiline);

                if (headerMatches.Count > 0)
                {
                    var headers = headerMatches.Cast<Match>().Select(m => m.Groups[1].Value).ToList();
                    metadata[$"Header_{level}"] = headers;
                }
            }

            var bolds = Regex.Matches(text, @"\*\*(.+?)\*\*");
            if (bolds.Count > 0)
            {
                metadata["Bold"] = bolds.Cast<Match>().Select(m => m.Groups[1].Value).ToList();
            }

            return metadata;
        }

        Dictionary<string, object> UpdateContext(Dictionary<string, object> currentMeta, Dictionary<string, object> previous)
        {
            var context = new Dictionary<string, object>(previous);
            var levels = Enumerable.Range(1, 6).Select(i => $"Header_{i}").Concat(new[] { "Bold" }).ToList();

            for (var i = 0; i < levels.Count; i++)
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
                else if (context.TryGetValue(key, out var val))
                {
                    if (val is List<string> list && list.Count > 0)
                    {
                        context[key] = new List<string> { list.Last() };
                    }
                }
            }

            return context;
        }

        var h1Sections = SplitByH1(markdown);
        var headerLevels = new[] { "##", "###", "####", "#####", "######" };
        var allChunks = new List<MarkdownChunk>();
        var context = new Dictionary<string, object>();

        foreach (var section in h1Sections)
        {
            var chunks = (countTokens(section) <= maxTokens)
                ? new List<string> { section }
                : RecursiveSplit(section, maxTokens, headerLevels);

            foreach (var chunk in chunks)
            {
                if (countTokens(chunk) < 30)
                {
                    continue;
                }

                var currentMeta = ExtractMetadata(chunk);
                context = UpdateContext(currentMeta, context);

                allChunks.Add(new MarkdownChunk
                {
                    Content = chunk.Trim(),
                    Metadata = new Dictionary<string, object>(context),
                    TokenCount = countTokens(chunk.Trim())
                });
            }
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };

        var json = JsonSerializer.Serialize(allChunks, options);
        Console.WriteLine(json);

        return allChunks;
    }
}