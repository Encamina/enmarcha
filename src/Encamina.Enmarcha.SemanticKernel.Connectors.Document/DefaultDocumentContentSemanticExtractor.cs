using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Default implementation of a document content semantic extractor.
/// </summary>
internal sealed class DefaultDocumentContentSemanticExtractor : DocumentContentSemanticExtractorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDocumentContentSemanticExtractor"/> class.
    /// </summary>
    /// <param name="semanticTextSplitter">A valid instance of <see cref="ISemanticTextSplitter"/> to use when extracting semantic content from documents.</param>
    /// <param name="embeddingsGeneratorFunction">An embeddings function to use when extracting semantic content from documents.</param>
    /// <param name="connectors">List of document connectors to register.</param>
    public DefaultDocumentContentSemanticExtractor(ISemanticTextSplitter semanticTextSplitter, Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> embeddingsGeneratorFunction, IEnumerable<IEnmarchaDocumentConnector> connectors)
        : base(semanticTextSplitter, embeddingsGeneratorFunction, connectors)
    {
    }
}
