namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <summary>
/// Provider interface to obtain instances of <see cref="IEnmarchaDocumentConnector"/>s.
/// </summary>
public interface IDocumentConnectorProvider
{
    /// <summary>
    /// Determines the most appropriate document connector from a specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <returns>A valid instance of <see cref="IEnmarchaDocumentConnector"/> that could handle documents from the given file extension.</returns>
    /// <exception cref="InvalidOperationException">
    /// If the <paramref name="fileExtension"/> is not supported or no suitable <see cref="IEnmarchaDocumentConnector"/> instance for it can be found.
    /// </exception>
    IEnmarchaDocumentConnector GetDocumentConnector(string fileExtension);

    /// <summary>
    /// Determines the most appropriate document connector from a specified file extension.
    /// </summary>
    /// <param name="fileExtension">The file extension.</param>
    /// <param name="throwException">
    /// If <see langword="true"/> an <see cref="InvalidOperationException"/> is thrown if the <paramref name="fileExtension"/> is not supported
    /// or no suitable <see cref="IEnmarchaDocumentConnector"/> instance for it can be found.
    /// </param>
    /// <returns>A valid instance of <see cref="IEnmarchaDocumentConnector"/> that could handle documents from the given file extension.</returns>
    /// <exception cref="InvalidOperationException">
    /// If the <paramref name="fileExtension"/> is not supported or no suitable <see cref="IEnmarchaDocumentConnector"/> instance for it can be found.
    /// </exception>
    IEnmarchaDocumentConnector? GetDocumentConnector(string fileExtension, bool throwException);

    /// <summary>
    /// Determines whether a specified file extension is supported.
    /// </summary>
    /// <param name="fileExtension">The file extension to check.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the file extension is supported; otherwise, <see langword="false"/>.
    /// </returns>
    bool SupportedFileExtension(string fileExtension);

    /// <summary>
    /// Adds a new document connector for a specified file extension.
    /// </summary>
    /// <remarks>
    /// If the file extension already has a document connector associated with it, the existing connector is replaced.
    /// </remarks>
    /// <param name="documentConnector">
    /// A valid instance of <see cref="IEnmarchaDocumentConnector"/> to handle documents with the specified file extension.
    /// </param>
    void AddDocumentConnector(IEnmarchaDocumentConnector documentConnector);
}
