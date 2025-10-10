using System.Text.Json.Nodes;

using CommunityToolkit.Diagnostics;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

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
    /// <param name="pages">The number of pages per split part.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of memory streams, each representing a part of the split PDF.</returns>
    /// <exception cref="ArgumentException">Thrown when pages is less than or equal to zero.</exception>
    public static Task<IReadOnlyList<MemoryStream>> SplitPdfByPagesAsync(Stream stream, int pages, CancellationToken ct)
    {
        Guard.IsNotNull(stream);

        if (pages <= 0)
        {
            throw new ArgumentException("Pages per part must be greater than zero.", nameof(pages));
        }

        var resultStreams = new List<MemoryStream>();

        try
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            using var sourcePdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            var totalPages = sourcePdf.PageCount;

            var currentPage = 0;
            var partNumber = 1;

            while (currentPage < totalPages)
            {
                ct.ThrowIfCancellationRequested();

                var targetPdf = new PdfDocument();
                var endPage = Math.Min(currentPage + pages, totalPages);

                for (var i = currentPage; i < endPage; i++)
                {
                    targetPdf.AddPage(sourcePdf.Pages[i]);
                }

                var partStream = new MemoryStream();
                targetPdf.Save(partStream, closeStream: false);
                partStream.Position = 0;

                resultStreams.Add(partStream);

                currentPage = endPage;
                partNumber++;
                targetPdf.Dispose();
            }

            return Task.FromResult<IReadOnlyList<MemoryStream>>(resultStreams.AsReadOnly());
        }
        catch (Exception ex)
        {
            foreach (var streamMemory in resultStreams)
            {
                stream.Dispose();
            }

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
        var pages = JsonNode.Parse(jsonContent)?["pages"]?.AsArray()
            ?? throw new InvalidOperationException("Invalid JSON structure: 'pages' array not found.");

        var markdownPages = pages
            .Select(page => page?["markdown"]?.GetValue<string>())
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
