using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <summary>
/// Represents a congnitive service that provides language detections capabilities.
/// </summary>
public interface ILanguageDetectionService : ICognitiveService
{
    /// <summary>
    /// Detects a language from a text.
    /// </summary>
    /// <param name="request">A language detection request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A result with detected languages.</returns>
    Task<LanguageDetectionResult> DetectLanguageAsync(LanguageDetectionRequest request, CancellationToken cancellationToken);
}
