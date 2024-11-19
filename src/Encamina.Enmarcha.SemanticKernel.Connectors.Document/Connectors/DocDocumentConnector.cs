using System.Text;

using CommunityToolkit.Diagnostics;

using NPOI.HWPF;
using NPOI.HWPF.Extractor;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a document in the <c>.doc</c> format.
/// </summary>
public class DocDocumentConnector : IEnmarchaDocumentConnector
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocDocumentConnector"/> class.
    /// </summary>
    public DocDocumentConnector()
    {
        // Register the code pages encoding provider for the .doc files
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".DOC"];

    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        var document = new HWPFDocument(stream);

        var extractor = new WordExtractor(document);

        return extractor.Text.Trim();
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
