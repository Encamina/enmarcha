using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Base class for document content extractors.
/// </summary>
public abstract class DocumentContentExtractorBase : IDocumentConnectorProvider, IDocumentContentExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentContentExtractorBase"/> class.
    /// </summary>
    /// <param name="textSplitter">A valid instance of <see cref="ITextSplitter"/> to use when extracting content from documents.</param>
    /// <param name="lengthFunction">A length function to use when extracting content from documents.</param>
    protected DocumentContentExtractorBase(ITextSplitter textSplitter, Func<string, int> lengthFunction)
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
    public abstract IDocumentConnector GetDocumentConnector(string fileExtension);
}
