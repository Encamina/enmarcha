using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class for document content extractors.
/// </summary>
public class DocumentContentExtractorBase : DocumentConnectorProviderBase, IDocumentContentExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentContentExtractorBase"/> class.
    /// </summary>
    /// <param name="textSplitter">A valid instance of <see cref="ITextSplitter"/> to use when extracting content from documents.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <param name="connectors">List of document connectors to register.</param>
    protected DocumentContentExtractorBase(ITextSplitter textSplitter, Func<string, int> lengthFunction, IEnumerable<IEnmarchaDocumentConnector> connectors) : base(connectors)
    {
        LengthFunction = lengthFunction;
        TextSplitter = textSplitter;
    }

    /// <summary>
    /// Gets the text splitter used by this instance of a document content extractor.
    /// </summary>
    protected ITextSplitter TextSplitter { get; }

    /// <summary>
    /// Gets the length function used by this instance of a document content extractor.
    /// </summary>
    protected Func<string, int> LengthFunction { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<string> GetDocumentContent(Stream stream, string fileExtension)
    {
        var connector = GetDocumentConnector(fileExtension);

        var content = connector.ReadText(stream);

        return TextSplitter.Split(content, LengthFunction);
    }

    /// <inheritdoc/>
    public virtual Task<IEnumerable<string>> GetDocumentContentAsync(Stream stream, string fileExtension, CancellationToken cancellationToken)
    {
        // Using Task.Run instead of Task.FromResult because the operation in GetDocumentContent is potentially slow,
        // and Task.Run ensures it is executed on a separate thread, maintaining responsiveness.
        return Task.Run(() => GetDocumentContent(stream, fileExtension), cancellationToken);
    }
}
