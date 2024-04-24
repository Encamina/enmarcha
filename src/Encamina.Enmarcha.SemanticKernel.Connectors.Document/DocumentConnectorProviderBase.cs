using System.Text;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Resources;

using Microsoft.SemanticKernel.Plugins.Document;
using Microsoft.SemanticKernel.Plugins.Document.OpenXml;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class to provide instances of <see cref="IDocumentConnector"/>s.
/// </summary>
public class DocumentConnectorProviderBase : IDocumentConnectorProvider
{
    private readonly Dictionary<string, IDocumentConnector> documentConnectors = new()
    {
        { @".DOCX", new WordDocumentConnector() },
        { @".PDF", new CleanPdfDocumentConnector() },
        { @".PPTX", new ParagraphPptxDocumentConnector() },
        { @".TXT", new TxtDocumentConnector(Encoding.UTF8) },
        { @".MD", new TxtDocumentConnector(Encoding.UTF8) },
        { @".VTT", new VttDocumentConnector(Encoding.UTF8) },
    };

    /// <inheritdoc/>
    public virtual void AddDocumentConnector(string fileExtension, IDocumentConnector documentConnector)
    {
        documentConnectors[fileExtension.ToUpperInvariant()] = documentConnector;
    }

    /// <inheritdoc/>
    public virtual IDocumentConnector GetDocumentConnector(string fileExtension)
    {
        return GetDocumentConnector(fileExtension, true);
    }

    /// <inheritdoc/>
    public IDocumentConnector GetDocumentConnector(string fileExtension, bool throwException)
    {
        if (documentConnectors.TryGetValue(fileExtension.ToUpperInvariant(), out var value))
        {
            return value;
        }

        if (throwException)
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentUICulture(nameof(ExceptionMessages.FileExtensionNotSupported), fileExtension));
        }

        return null;
    }

    /// <inheritdoc/>
    public virtual bool SupportedFileExtension(string fileExtension)
    {
        return documentConnectors.ContainsKey(fileExtension.ToUpperInvariant());
    }
}
