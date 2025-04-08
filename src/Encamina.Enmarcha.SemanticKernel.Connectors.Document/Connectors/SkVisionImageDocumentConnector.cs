using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text (OCR) and interprets information from images, diagrams and unstructured information. Uses Semantic Kernel.
/// </summary>
public class SkVisionImageDocumentConnector : SkVisionImageExtractor, IEnmarchaDocumentConnector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SkVisionImageDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options">Configuration options for this connector.</param>
    public SkVisionImageDocumentConnector(Kernel kernel, IOptions<SkVisionImageDocumentConnectorOptions> options)
        : base(kernel, options)
    {
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".JPEG", ".JPG", ".PNG"];

    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);
        return ProcessImageWithVision(stream);
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
