using Microsoft.SemanticKernel.Plugins.Document;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document;

/// <inheritdoc/>
public interface IEnmarchaDocumentConnector : IDocumentConnector
{
    /// <summary>
    /// Gets the list of compatible file formats.
    /// </summary>
    /// <remarks>
    /// The file forms should be in the format ".{extension}". In upper case. For example, ".DOCX".
    /// </remarks>
    IReadOnlyList<string> CompatibleFileFormats { get; }
}
