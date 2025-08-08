using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Utils;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <inheritdoc/>
public class SkVisionWordDocumentConnector : IEnmarchaDocumentConnector
{
    private readonly SkVisionImageExtractor? imageExtractor;

    /// <summary>
    /// Initializes a new instance of the <see cref="SkVisionWordDocumentConnector"/> class.
    /// </summary>
    /// <param name="kernel">A valid <see cref="Kernel"/> instance.</param>
    /// <param name="options">Configuration options for vision processing.</param>
    public SkVisionWordDocumentConnector(Kernel kernel, IOptions<SkVisionImageExtractorOptions> options)
    {
        Guard.IsNotNull(kernel);

        this.imageExtractor = options?.Value is not null ? new SkVisionImageExtractor(kernel, options) : null;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".DOCX"];

    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        return DocxExtractorHelper.ExtractText(stream, imageExtractor);
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
