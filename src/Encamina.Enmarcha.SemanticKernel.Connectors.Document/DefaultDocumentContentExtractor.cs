using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Default implementation of a document content extractor.
/// </summary>
internal sealed class DefaultDocumentContentExtractor : DocumentContentExtractorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDocumentContentExtractor"/> class.
    /// </summary>
    /// <param name="textSplitter">The text splitter used by this instance.</param>
    /// <param name="lengthFunction">The function for determining the length of a string.</param>
    /// <param name="connectors">List of document connectors to register.</param>
    public DefaultDocumentContentExtractor(ITextSplitter textSplitter, Func<string, int> lengthFunction, IEnumerable<IEnmarchaDocumentConnector> connectors) : base(textSplitter, lengthFunction, connectors)
    {
    }
}
