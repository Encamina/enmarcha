using System.Text;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Resources;

using Microsoft.SemanticKernel.Plugins.Document;
using Microsoft.SemanticKernel.Plugins.Document.OpenXml;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Default implementation of a document content extractor.
/// </summary>
/// <remarks>
/// This default implementation supports the following file extensions:
/// <list type="bullet">
///     <item><c>docx</c></item>
///     <item><c>md</c></item>
///     <item><c>pdf</c></item>
///     <item><c>pptx</c></item>
///     <item><c>txt</c></item>
///     <item><c>vtt</c></item>
/// </list>
/// </remarks>
internal sealed class DefaultDocumentContentExtractor : DocumentContentExtractorBase
{
    public DefaultDocumentContentExtractor(ITextSplitter textSplitter, Func<string, int> lengthFunction) : base(textSplitter, lengthFunction)
    {
    }

    /// <inheritdoc/>
    protected override IDocumentConnector GetDocumentConnector(string fileExtension)
    {
        return fileExtension.ToUpperInvariant() switch
        {
            @".DOCX" => new WordDocumentConnector(),
            @".PDF" => new CleanPdfDocumentConnector(),
            @".PPTX" => new ParagraphPptxDocumentConnector(),
            @".TXT" => new TxtDocumentConnector(Encoding.UTF8),
            @".MD" => new TxtDocumentConnector(Encoding.UTF8),
            @".VTT" => new VttDocumentConnector(Encoding.UTF8),
            _ => throw new NotSupportedException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ExceptionMessages.FileExtensionNotSupported), fileExtension)),
        };
    }
}
