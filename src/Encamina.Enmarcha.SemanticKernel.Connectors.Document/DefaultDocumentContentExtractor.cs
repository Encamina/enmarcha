using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.SemanticKernel.Plugins.Document;

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
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDocumentContentExtractor"/> class.
    /// </summary>
    /// <param name="textSplitter">The text splitter used by this instance.</param>
    /// <param name="lengthFunction">The function for determining the length of a string.</param>
    public DefaultDocumentContentExtractor(ITextSplitter textSplitter, Func<string, int> lengthFunction) : base(textSplitter, lengthFunction)
    {
    }
}
