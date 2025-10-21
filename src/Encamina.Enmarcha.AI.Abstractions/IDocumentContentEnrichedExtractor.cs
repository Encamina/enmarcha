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
    IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)> GetDocumentContentWithMetadata(Stream stream, string fileExtension);

    /// <summary>
    /// Asynchronously extracts the content from a document stream.
    /// </summary>
    /// <param name="stream">The document stream.</param>
    /// <param name="fileExtension">The extension of the document file.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The text content of the document with metadata.</returns>
    Task<IEnumerable<(IReadOnlyDictionary<string, string> Metadata, string Text)>> GetDocumentContentWithMetadataAsync(Stream stream, string fileExtension, CancellationToken cancellationToken);
}
