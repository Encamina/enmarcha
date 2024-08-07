using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class for document content semantic extractors.
/// </summary>
public abstract class DocumentContentSemanticExtractorBase : DocumentConnectorProviderBase, IDocumentContentExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentContentSemanticExtractorBase"/> class.
    /// </summary>
    /// <param name="semanticTextSplitter">A valid instance of <see cref="ISemanticTextSplitter"/> to use when extracting semantic content from documents.</param>
    /// <param name="embeddingsGeneratorFunction">An embeddings function to use when extracting semantic content from documents.</param>
    protected DocumentContentSemanticExtractorBase(ISemanticTextSplitter semanticTextSplitter, Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> embeddingsGeneratorFunction)
    {
        SemanticTextSplitter = semanticTextSplitter;
        EmbeddingsGeneratorFunction = embeddingsGeneratorFunction;
    }

    /// <summary>
    /// Gets the text semantic splitter used by this instance of a document content extractor.
    /// </summary>
    protected ISemanticTextSplitter SemanticTextSplitter { get; }

    /// <summary>
    /// Gets the function for generating embeddings from a list of strings.
    /// </summary>
    protected Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> EmbeddingsGeneratorFunction { get; }

    /// <inheritdoc/>
    /// <remarks>Consider using the asynchronous version directly to avoid the risk of blocking the calling thread.</remarks>
    public IEnumerable<string> GetDocumentContent(Stream stream, string fileExtension)
    {
        return GetDocumentContentAsync(stream, fileExtension, default).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public virtual Task<IEnumerable<string>> GetDocumentContentAsync(Stream stream, string fileExtension, CancellationToken cancellationToken)
    {
        var connector = GetDocumentConnector(fileExtension);

        var content = connector.ReadText(stream);

        return SemanticTextSplitter.SplitAsync(content, EmbeddingsGeneratorFunction, cancellationToken);
    }
}