namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <inheritdoc/>
public class WordDocumentConnector : Microsoft.SemanticKernel.Plugins.Document.OpenXml.WordDocumentConnector, IEnmarchaDocumentConnector
{
    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".DOCX"];
}
