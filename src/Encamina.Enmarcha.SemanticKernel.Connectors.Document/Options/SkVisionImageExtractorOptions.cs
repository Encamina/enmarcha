namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

/// <summary>
/// Configuration options for <see cref="SkVisionImageExtractor"/>.
/// </summary>
public class SkVisionImageExtractorOptions
{
    /// <summary>
    /// Gets the resolution limit (in pixels) for images.
    /// </summary>
    public int ResolutionLimit { get; init; } = 8192;
}