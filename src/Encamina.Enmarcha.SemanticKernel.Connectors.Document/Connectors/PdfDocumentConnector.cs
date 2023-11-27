// Ignore Spelling: Pdf

using CommunityToolkit.Diagnostics;

using Microsoft.SemanticKernel.Plugins.Document;

using UglyToad.PdfPig;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a Portable Document File (<c>.pdf</c>).
/// </summary>
/// <remarks>
/// During the text extraction process, it's important to note that the generated text for each section does not preserve the original structure
/// of the PDF document, such as paragraphs and other formatting elements. Instead, a single paragraph is generated for each page. This means that
/// each page generates text as if it were a single paragraph, and line breaks are added between different pages for separation.
/// For strict format text extraction, consider using <see cref="StrictFormatCleanPdfDocumentConnector"/>.
/// </remarks>
public class PdfDocumentConnector : IDocumentConnector
{
    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var document = PdfDocument.Open(stream);

        var pagesText = document.GetPages().Select(p => p.Text);

        return string.Join(Environment.NewLine, pagesText).Trim();
    }

    /// <inheritdoc/>
    public virtual void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public virtual void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }
}