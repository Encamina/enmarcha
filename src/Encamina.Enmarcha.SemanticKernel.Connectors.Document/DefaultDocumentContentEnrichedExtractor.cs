using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Default implementation of an enriched document content extractor.
/// </summary>
internal sealed class DefaultDocumentContentEnrichedExtractor : DocumentContentEnrichedExtractorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultDocumentContentEnrichedExtractor"/> class.
    /// </summary>
    /// <param name="enrichedTextSplitter">The enriched text splitter used by this instance.</param>
    /// <param name="lengthFunction">The function for determining the length of a string.</param>
    /// <param name="connectors">List of document connectors to register.</param>
    public DefaultDocumentContentEnrichedExtractor(IEnrichedTextSplitter enrichedTextSplitter, Func<string, int> lengthFunction, IEnumerable<IEnmarchaDocumentConnector> connectors) : base(enrichedTextSplitter, lengthFunction, connectors)
    {
    }
}
