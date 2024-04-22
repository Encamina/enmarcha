using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Default implementation of a document content semantic extractor.
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
internal sealed class DefaultDocumentContentSemanticExtractor : DocumentContentSemanticExtractorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDocumentContentSemanticExtractor"/> class.
    /// </summary>
    /// <param name="semanticTextSplitter">A valid instance of <see cref="ISemanticTextSplitter"/> to use when extracting semantic content from documents.</param>
    /// <param name="embeddingsGeneratorFunction">An embeddings function to use when extracting semantic content from documents.</param>
    public DefaultDocumentContentSemanticExtractor(ISemanticTextSplitter semanticTextSplitter, Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> embeddingsGeneratorFunction) : base(semanticTextSplitter, embeddingsGeneratorFunction)
    {
    }
}
