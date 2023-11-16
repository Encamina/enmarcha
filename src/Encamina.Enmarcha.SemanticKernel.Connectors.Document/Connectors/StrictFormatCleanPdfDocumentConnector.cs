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
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        using var document = PdfDocument.Open(stream);

        var textBlocks = new List<TextBlock>();
        foreach (var page in document.GetPages())
        {
            var pageTextBlocks = GetTextBlocks(page);

            textBlocks.AddRange(pageTextBlocks);
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

    private static IEnumerable<TextBlock> GetTextBlocks(Page page)
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

        // 3. Postprocessing
        var orderedTextBlocks = RenderingReadingOrderDetector.Instance.Get(textBlocks);

        return orderedTextBlocks;
    }

    private static IEnumerable<TextBlock> CleanTextBlocks(PdfDocument document, IEnumerable<TextBlock> textBlocks)
    {
        var horizontalTextBlocks = textBlocks
            .Where(tb => tb.TextOrientation == TextOrientation.Horizontal)
            .Select(textBlock => new TextBlockElement(textBlock));

        return RemoveCommonTextElements(document, horizontalTextBlocks);
    }
}
