using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Resources;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class to provide instances of <see cref="IDocumentConnector"/>s.
/// </summary>
public class DocumentConnectorProviderBase : IDocumentConnectorProvider
{
    private readonly Dictionary<string, IEnmarchaDocumentConnector> documentConnectors;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentConnectorProviderBase"/> class.
    /// </summary>
    /// <param name="connectors">List of document connectors to register.</param>
    public DocumentConnectorProviderBase(IEnumerable<IEnmarchaDocumentConnector> connectors)
    {
        documentConnectors = [];

        foreach (var connector in connectors)
        {
            foreach (var fileExtension in connector.CompatibleFileFormats)
            {
                documentConnectors[fileExtension.ToUpperInvariant()] = connector;
            }
        }
    }

    /// <inheritdoc/>
    public virtual void AddDocumentConnector(IEnmarchaDocumentConnector documentConnector)
    {
        foreach (var fileExtension in documentConnector.CompatibleFileFormats)
        {
            documentConnectors[fileExtension.ToUpperInvariant()] = documentConnector;
        }
    }

    /// <inheritdoc/>
    public virtual IEnmarchaDocumentConnector GetDocumentConnector(string fileExtension)
    {
        return GetDocumentConnector(fileExtension, true)!;
    }

    /// <inheritdoc/>
    public IEnmarchaDocumentConnector? GetDocumentConnector(string fileExtension, bool throwException)
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
