using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

/// <summary>
/// Configuration options for <see cref="SkVisionImageDocumentConnector"/>.
/// </summary>
public sealed class SkVisionImageDocumentConnectorOptions
{
    /// <summary>
    /// Gets the resolution limit (in pixels) for images.
    /// </summary>
    public int ResolutionLimit { get; init; } = 8192;
}
