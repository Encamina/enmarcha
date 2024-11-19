using CommunityToolkit.Diagnostics;

using HtmlAgilityPack;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

/// <summary>
/// Extracts text from a document in the <c>.html</c> format.
/// </summary>
public class HtmlDocumentConnector : IEnmarchaDocumentConnector
{
    /// <inheritdoc/>
    public IReadOnlyList<string> CompatibleFileFormats => [".HTML"];

    /// <inheritdoc/>
    public virtual string ReadText(Stream stream)
    {
        Guard.IsNotNull(stream);

        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(stream);

        var text = htmlDoc.DocumentNode.InnerText.Trim();

        // Remove all html tags from the text
        var cleanedText = HtmlEntity.DeEntitize(text);

        return cleanedText;
    }

    /// <inheritdoc/>
    public virtual void Initialize(Stream stream)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }

    /// <inheritdoc/>
    public virtual void AppendText(Stream stream, string text)
    {
        // Intentionally not implemented to comply with the Liskov Substitution Principle...
    }
}
