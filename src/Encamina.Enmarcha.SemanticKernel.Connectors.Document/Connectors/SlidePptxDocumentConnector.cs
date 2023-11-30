// Ignore Spelling: pptx

using System.Text;

using CommunityToolkit.Diagnostics;

using DocumentFormat.OpenXml.Packaging;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts the text from a Microsoft PowerPoint (<c>.pptx</c>) file, just one line for each slide found.
/// </summary>
internal sealed class SlidePptxDocumentConnector : BasePptxDocumentConnector
{
    /// <inheritdoc/>
    protected override IEnumerable<string> GetAllTextInSlide(SlidePart slidePart)
    {
        Guard.IsNotNull(slidePart);

        var slideText = new StringBuilder();

        // Iterate through all the paragraphs in the slide.
        foreach (var paragraph in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
        {
            // Iterate through the lines of the paragraph.
            foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
            {
                slideText.Append(text.Text).Append(@". ");
            }
        }

        return new[] { slideText.ToString().Trim() };
    }
}
