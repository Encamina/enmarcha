using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class for enriched document content extractors.
/// </summary>
public abstract class DocumentContentEnrichedExtractorBase : DocumentConnectorProviderBase, IDocumentContentEnrichedExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentContentEnrichedExtractorBase"/> class.
    /// </summary>
    /// <param name="enrichedTextSplitter">A valid instance of <see cref="IEnrichedTextSplitter"/> to use when extracting content from documents.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    /// <param name="connectors">List of document connectors to register.</param>
    protected DocumentContentEnrichedExtractorBase(IEnrichedTextSplitter enrichedTextSplitter, Func<string, int> lengthFunction, IEnumerable<IEnmarchaDocumentConnector> connectors) : base(connectors)
    {
         LengthFunction = lengthFunction;
         EnrichedTextSplitter = enrichedTextSplitter;
    }

    /// <summary>
    /// Gets the text semantic splitter used by this instance of a document content extractor.
    /// </summary>
    protected IEnrichedTextSplitter EnrichedTextSplitter { get; }

    /// <summary>
    /// Gets the length function used by this instance of a document content extractor.
    /// </summary>
    protected Func<string, int> LengthFunction { get; }

    /// <inheritdoc/>
    public virtual IEnumerable<(IDictionary<string, string> Metadata, string Text)> GetDocumentContentWithMetadata(Stream stream, string fileExtension)
    {
        var connector = GetDocumentConnector(fileExtension);

        var content = connector.ReadText(stream);

        return EnrichedTextSplitter.SplitWithMetadata(content, LengthFunction);
    }

    /// <inheritdoc/>
    public virtual Task<IEnumerable<(IDictionary<string, string> Metadata, string Text)>> GetDocumentContentWithMetadataAsync(Stream stream, string fileExtension, CancellationToken cancellationToken)
    {
        // Using Task.Run instead of Task.FromResult because the operation in GetDocumentContentWithMetadata is potentially slow,
        // and Task.Run ensures it is executed on a separate thread, maintaining responsiveness.
        return Task.Run(() => GetDocumentContentWithMetadata(stream, fileExtension), cancellationToken);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<string> GetDocumentContent(Stream stream, string fileExtension)
    {
        var connector = GetDocumentConnector(fileExtension);

        var content = connector.ReadText(stream);

        return EnrichedTextSplitter.Split(content, LengthFunction);
    }

    /// <inheritdoc/>
    public virtual Task<IEnumerable<string>> GetDocumentContentAsync(Stream stream, string fileExtension, CancellationToken cancellationToken)
    {
        // Using Task.Run instead of Task.FromResult because the operation in GetDocumentContent is potentially slow,
        // and Task.Run ensures it is executed on a separate thread, maintaining responsiveness.
        return Task.Run(() => GetDocumentContent(stream, fileExtension), cancellationToken);
    }
}
