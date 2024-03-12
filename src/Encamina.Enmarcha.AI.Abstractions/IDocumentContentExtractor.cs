namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents a document content extractor, which extracts its content from a stream.
/// </summary>
public interface IDocumentContentExtractor
{
    /// <summary>
    /// Extracts the content from a document stream.
    /// </summary>
    /// <param name="stream">The document stream.</param>
    /// <param name="fileExtension">The extension of the document file.</param>
    /// <returns>The text content of the document.</returns>
    IEnumerable<string> GetDocumentContent(Stream stream, string fileExtension);

    /// <summary>
    /// Asynchronously extracts the content from a document stream.
    /// </summary>
    /// <param name="stream">The document stream.</param>
    /// <param name="fileExtension">The extension of the document file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The text content of the document.</returns>
    Task<IEnumerable<string>> GetDocumentContentAsync(Stream stream, string fileExtension, CancellationToken cancellationToken);
}
