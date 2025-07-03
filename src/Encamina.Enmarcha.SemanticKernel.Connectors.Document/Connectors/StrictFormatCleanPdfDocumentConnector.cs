// Ignore Spelling: pdf

using System.Text;

using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document Format (<c>.pdf</c>) file, attempts to preserve the document's formatting, including paragraphs, titles, and other structural elements,
/// while also removing common words that overlap across pages and excluding non-horizontal text.
/// </summary>
/// <remarks>
/// During the text extraction process, an effort is made to retain the document's format, which includes preserving the structure of paragraphs, titles, and other elements.
/// However, it's important to note that this process relies on OCR recognition, which is not perfect, and the results may vary depending on the quality of the PDF.
/// </remarks>
public class StrictFormatCleanPdfDocumentConnector : CleanPdfDocumentConnector
{
    /// <inheritdoc/>
    public override string ReadText(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        using var document = PdfDocument.Open(stream);

        var textBlocks = new List<TextBlock>();
        foreach (var page in document.GetPages())
        {
            textBlocks.AddRange(GetTextBlocks(page));

            textBlocks.AddRange(ProcessPageExtensions(page));
        }

        var cleanedTextBlocks = CleanTextBlocks(document, textBlocks);

        var sb = new StringBuilder();
        foreach (var block in cleanedTextBlocks)
        {
            sb.Append(block.Text);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extracts <see cref="TextBlock"/> from a given page of a PDF document.
    /// </summary>
    /// <param name="page">The page from which to extract text blocks.</param>
    /// <returns>An enumerable collection of <see cref="TextBlock" /> extracted from the page.</returns>
    protected static IEnumerable<TextBlock> GetTextBlocks(Page page)
    {
        // 1. Extract words
        var words = NearestNeighbourWordExtractor.Instance.GetWords(page.Letters);

        // 2. Segment page
        var pageSegmenterOptions = new DocstrumBoundingBoxes.DocstrumBoundingBoxesOptions()
        {
            LineSeparator = " ",
        };
        var pageSegmenter = new DocstrumBoundingBoxes(pageSegmenterOptions);
        var textBlocks = pageSegmenter.GetBlocks(words);

        // 3. Post-processing
        var orderedTextBlocks = RenderingReadingOrderDetector.Instance.Get(textBlocks);

        return orderedTextBlocks;
    }

    /// <summary>
    /// Cleans the extracted text blocks from a PDF document by removing common words that overlap across pages and excluding non-horizontal text.
    /// </summary>
    /// <param name="document">The PDF document.</param>
    /// <param name="textBlocks">The extracted text blocks.</param>
    /// <returns>The cleaned text blocks.</returns>
    protected static IEnumerable<TextBlock> CleanTextBlocks(PdfDocument document, IEnumerable<TextBlock> textBlocks)
    {
        var horizontalTextBlocks = textBlocks
            .Where(tb => tb.TextOrientation == TextOrientation.Horizontal)
            .Select(textBlock => new TextBlockElement(textBlock));

        return RemoveCommonTextElements(document, horizontalTextBlocks);
    }

    /// <summary>
    /// Extension point to process a page and append additional text blocks to the extracted content.
    /// This method can be overridden in derived classes to provide custom processing for specific pages.
    /// </summary>
    /// <param name="page">The PDF page to process.</param>
    /// <returns>Additional <see cref="TextBlock"/> to append to the page content.</returns>
    protected virtual IEnumerable<TextBlock> ProcessPageExtensions(Page page)
    {
        return [];
    }
}
