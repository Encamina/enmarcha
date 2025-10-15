using System.Text;
using System.Text.Json.Nodes;

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

        var pages = JsonNode.Parse(jsonContent)?["pages"]?.AsArray() ?? throw new InvalidOperationException("Invalid JSON structure: 'pages' array not found.");

        var extractedMarkdown = new StringBuilder();

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
            extractedMarkdown.Append(processedMarkdown);
            extractedMarkdown.Append("\n\n");
        }

        return extractedMarkdown.ToString();
    }

    /// <summary>
    /// Split the markdown into chunks according to the maximum number of characters for LLM processing.
    /// </summary>
    /// <param name="markdown">The markdown text to be split.</param>
    /// <param name="maxTokens">Maximum number of tokens per chunk.</param>
    /// <param name="lengthFunction">Function to count tokens in a string.</param>
    /// <returns>List of markdown chunks.</returns>
    public static List<string> SplitMarkdownForRefinement(string markdown, int maxTokens, Func<string, int> lengthFunction)
    {
        var parts = new List<string>();
        var current = new StringBuilder();

        maxTokens /= 2;

        var lines = markdown.Split('\n');

        foreach (var line in lines)
        {
            var lineWithNewline = line + "\n";
            var tentative = current.ToString() + lineWithNewline;

            var tentativeTokenCount = lengthFunction(tentative);

            if (tentativeTokenCount > maxTokens && current.Length > 0)
            {
                parts.Add(current.ToString());
                current.Clear();
                current.Append(lineWithNewline);
            }
            else
            {
                current.Append(lineWithNewline);
            }
        }

        if (current.Length > 0)
        {
            parts.Add(current.ToString());
        }

        return parts;
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
}
