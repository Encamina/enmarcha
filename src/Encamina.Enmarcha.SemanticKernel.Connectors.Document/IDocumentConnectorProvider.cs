using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Provider interface to obtain instances of <see cref="IDocumentConnector"/>s.
/// </summary>
public interface IDocumentConnectorProvider
{
    /// <summary>
    /// Determines the most appropriate document connector from an specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns>A valid instance of <see cref="IDocumentConnector"/> that could handle documents from the given file extension.</returns>
    protected abstract IDocumentConnector GetDocumentConnector(string fileExtension);
}
