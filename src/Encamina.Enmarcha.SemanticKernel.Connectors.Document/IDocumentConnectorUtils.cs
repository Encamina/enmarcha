using System.Text;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Resources;

using Microsoft.SemanticKernel.Plugins.Document;
using Microsoft.SemanticKernel.Plugins.Document.OpenXml;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Utility class providing methods for working with document connectors.
/// </summary>
public static class IDocumentConnectorUtils
{
    /// <summary>
    /// Gets the default document connector based on the specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension for which to retrieve the connector.</param>
    /// <returns>An instance of the default document connector for the specified file extension.</returns>
    /// <exception cref="NotSupportedException">Thrown when the file extension is not supported.</exception>
    public static IDocumentConnector GetDefaultDocumentConnector(string fileExtension)
    {
        return fileExtension.ToUpperInvariant() switch
        {
            @".DOCX" => new WordDocumentConnector(),
            @".PDF" => new CleanPdfDocumentConnector(),
            @".PPTX" => new ParagraphPptxDocumentConnector(),
            @".TXT" => new TxtDocumentConnector(Encoding.UTF8),
            @".MD" => new TxtDocumentConnector(Encoding.UTF8),
            @".VTT" => new VttDocumentConnector(Encoding.UTF8),
            @".XLSX" => new ExcelToMarkdownDocumentConnector(),
            _ => throw new NotSupportedException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ExceptionMessages.FileExtensionNotSupported), fileExtension)),
        };
    }
}
