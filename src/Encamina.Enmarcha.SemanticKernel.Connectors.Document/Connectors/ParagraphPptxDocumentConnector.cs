using System.Text;

using CommunityToolkit.Diagnostics;

using DocumentFormat.OpenXml.Packaging;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts the text from a Microsoft PowerPoint (<c>.pptx</c>) file, one line per paragraph found in each slide.
/// </summary>
public sealed class ParagraphPptxDocumentConnector : BasePptxDocumentConnector
{
    /// <inheritdoc/>
    protected override IEnumerable<string> GetAllTextInSlide(SlidePart slidePart)
    {
        Guard.IsNotNull(slidePart);

        var slideTexts = new List<string>();

        // Iterate through all the paragraphs in the slide.
        foreach (var paragraph in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
        {
            var paragraphText = new StringBuilder();

            // Iterate through the lines of the paragraph.
            foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
            {
                paragraphText.Append(text.Text);
            }

            if (paragraphText.Length > 0)
            {
                slideTexts.Add(paragraphText.ToString());
            }
        }

        return slideTexts;
    }
}
