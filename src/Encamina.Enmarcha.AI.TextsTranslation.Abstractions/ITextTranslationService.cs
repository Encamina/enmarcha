using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents a congnitive service that provides text translation capabilities.
/// </summary>
public interface ITextTranslationService : ICognitiveService
{
    /// <summary>
    /// Translates text to other languages.
    /// </summary>
    /// <param name="request">A text translation request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A result with the translated texts.</returns>
    Task<TextTranslationResult> TranslateAsync(TextTranslationRequest request, CancellationToken cancellationToken);
}
