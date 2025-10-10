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
    public static List<string> SplitMarkdown(string markdown, int maxChunkSize = 5000)
    {
        var parts = new List<string>();
        var current = new List<string>();
        var length = 0;

        using (var reader = new StringReader(markdown))
        {
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var lineWithEnding = line + Environment.NewLine;

                if (length + lineWithEnding.Length > maxChunkSize)
                {
                    parts.Add(string.Join(string.Empty, current));
                    current = [lineWithEnding];
                    length = lineWithEnding.Length;
                }
                else
                {
                    current.Add(lineWithEnding);
                    length += lineWithEnding.Length;
                }
            }
        }

        if (current.Count > 0)
        {
            parts.Add(string.Join(string.Empty, current));
        }

        return parts;
    }
}
