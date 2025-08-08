using System.Text;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

/// <summary>
/// A class for extracting content from DOCX files.
/// </summary>
internal static class DocxExtractorHelper
{
    /// <summary>
    /// Extracts text from a DOCX file stream.
    /// If use a <see cref="SkVisionImageExtractor"/>, it will process images in the document, otherwise it will ignore them.
    /// </summary>
    /// <param name="docxStream">Stream of the DOCX file.</param>
    /// <param name="imageExtractor">Semantic Kernel image extractor for processing images, can be null.</param>
    /// <returns>Extracted text with images and tables.</returns>
    public static string ExtractText(Stream docxStream, SkVisionImageExtractor? imageExtractor = null)
    {
        var sb = new StringBuilder();
        using var doc = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(docxStream, false);

        if (doc.MainDocumentPart is null)
        {
            return sb.ToString();
        }

        var body = doc.MainDocumentPart?.Document?.Body ?? new DocumentFormat.OpenXml.Wordprocessing.Body();
        foreach (var element in body.Elements())
        {
            if (element is DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph)
            {
                var paragraphText = ProcessParagraph(paragraph, doc.MainDocumentPart!, imageExtractor);
                sb.AppendLine(paragraphText);
            }
            else if (element is DocumentFormat.OpenXml.Wordprocessing.Table table)
            {
                var tableText = ProcessTable(table);
                sb.AppendLine(tableText);
            }
        }

        return sb.ToString();
    }

    private static string ProcessParagraph(
        DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph,
        DocumentFormat.OpenXml.Packaging.MainDocumentPart mainDocumentPart,
        SkVisionImageExtractor? imageExtractor = null)
    {
        var sb = new StringBuilder();

        foreach (var run in paragraph.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
        {
            if (imageExtractor is not null && run.Descendants<DocumentFormat.OpenXml.Wordprocessing.Drawing>().Any())
            {
                foreach (var drawing in run.Descendants<DocumentFormat.OpenXml.Wordprocessing.Drawing>())
                {
                    var blip = drawing.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                    if (blip?.Embed?.Value == null)
                    {
                        continue;
                    }

                    var imagePart = (DocumentFormat.OpenXml.Packaging.ImagePart)mainDocumentPart.GetPartById(blip.Embed.Value);

                    // Using a System.Drawing.Image to convert the image
                    using var imagePartStream = imagePart.GetStream(FileMode.Open, FileAccess.Read);
                    using var image = System.Drawing.Image.FromStream(imagePartStream);

                    // Convert to PNG before process with SK Vision
                    using var pngStream = new MemoryStream();
                    image.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);

                    sb.AppendLine(imageExtractor.ProcessImageWithVision(pngStream));
                }

                continue;
            }

            var textElement = run.GetFirstChild<DocumentFormat.OpenXml.Wordprocessing.Text>();
            if (textElement != null)
            {
                sb.Append(string.Format(@"{0} ", textElement.Text));
            }

            if (run.Descendants<DocumentFormat.OpenXml.Wordprocessing.Break>().Any())
            {
                sb.AppendLine();
            }
        }

        foreach (var link in paragraph.Elements<DocumentFormat.OpenXml.Wordprocessing.Hyperlink>())
        {
            var linkText = string.Concat(link.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>()?.Select(t => t.Text) ?? []);
            var linkUrl = string.Empty;
            if (link.Id != null)
            {
                var rel = mainDocumentPart.HyperlinkRelationships.FirstOrDefault(r => r.Id == link.Id);
                linkUrl = rel?.Uri?.ToString() ?? string.Empty;
            }
            else if (link.Anchor != null)
            {
                linkUrl = "#" + link.Anchor;
            }

            sb.AppendLine(@"[HYPERLINK]");
            sb.AppendLine(string.Format(@"Description: {0}; Url: {1}", linkText, linkUrl));
            sb.AppendLine(@"[/HYPERLINK]");
        }

        return sb.ToString();
    }

    private static string ProcessTable(DocumentFormat.OpenXml.Wordprocessing.Table table)
    {
        var sb = new StringBuilder();
        sb.AppendLine(@"[TABLE]");

        var rows = table.Elements<DocumentFormat.OpenXml.Wordprocessing.TableRow>();
        foreach (var row in rows)
        {
            var cellsText = new List<string>();

            var cells = row.Elements<DocumentFormat.OpenXml.Wordprocessing.TableCell>();
            foreach (var cell in cells)
            {
                cellsText.Add(string.Join(@" ", cell.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>().Select(p => p.InnerText.Trim())));
            }

            sb.AppendLine(string.Join(@" | ", cellsText));
        }

        sb.AppendLine(@"[/TABLE]");

        return sb.ToString();
    }
}
