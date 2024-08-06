// Ignore Spelling: txt

using System.Text;

using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extract text from a text (<c>.txt</c>) file.
/// </summary>
public class TxtDocumentConnector : IEnmarchaDocumentConnector
{
    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".TXT", ".MD"];

    /// <summary>
    /// Gets the encoding used for reading the text from the stream.
    /// </summary>
    protected virtual Encoding Encoding => Encoding.UTF8;

    /// <inheritdoc/>
    public string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        using var reader = new StreamReader(stream, Encoding);
        return reader.ReadToEnd();
    }

    /// <inheritdoc/>
    public void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }
}
