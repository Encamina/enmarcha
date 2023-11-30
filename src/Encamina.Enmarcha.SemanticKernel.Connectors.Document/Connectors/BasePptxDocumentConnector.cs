// Ignore Spelling: pptx

using System.Text;

using CommunityToolkit.Diagnostics;

using DocumentFormat.OpenXml.Packaging;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Base abstract class that defines a Microsoft PowerPoint (<c>.pptx</c>) document connector.
/// </summary>
public abstract class BasePptxDocumentConnector : IDocumentConnector
{
    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var presentation = PresentationDocument.Open(stream, false);

        var slideParts = presentation.PresentationPart?.SlideParts;

        if (slideParts == null)
        {
            return string.Empty;
        }

        var textBuilder = new StringBuilder();

        foreach (var slidePart in slideParts)
        {
            var slideText = GetAllTextInSlide(slidePart).Where(t => !string.IsNullOrWhiteSpace(t));

            if (slideText.Any())
            {
                textBuilder.AppendLine(string.Join(' ', slideText));
                textBuilder.AppendLine(string.Empty);
            }
        }

        return textBuilder.ToString().Trim();
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

    /// <summary>
    /// Gets the text from the specified <paramref name="slidePart"/>.
    /// </summary>
    /// <param name="slidePart">The slide part to extract text from.</param>
    /// <returns>A collection of strings representing the texts extracted from <paramref name="slidePart"/>.</returns>
    protected abstract IEnumerable<string> GetAllTextInSlide(SlidePart slidePart);
}
