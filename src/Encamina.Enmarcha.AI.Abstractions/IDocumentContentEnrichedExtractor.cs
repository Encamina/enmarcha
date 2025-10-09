namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents an enriched document content extractor, which extracts its content from a stream.
/// </summary>
public interface IDocumentContentEnrichedExtractor : IDocumentContentExtractor
{
    /// <summary>
    /// Extracts the content from a document stream.
    /// </summary>
    /// <param name="stream">The document stream.</param>
    /// <param name="fileExtension">The extension of the document file.</param>
    /// <returns>The text content of the document with metadata.</returns>
    new IEnumerable<(IDictionary<string, string> Metadata, string Text)> GetDocumentContent(Stream stream, string fileExtension);

    /// <summary>
    /// Asynchronously extracts the content from a document stream.
    /// </summary>
    /// <param name="stream">The document stream.</param>
    /// <param name="fileExtension">The extension of the document file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The text content of the document with metadata.</returns>
    new Task<IEnumerable<(IDictionary<string, string> Metadata, string Text)>> GetDocumentContentAsync(Stream stream, string fileExtension, CancellationToken cancellationToken);
}
