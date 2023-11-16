namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <inheritdoc/>
public class LanguageDetectionResult : ILanguageDetectionResult
{
    /// <inheritdoc/>
    public virtual IEnumerable<DetectedLanguage> DetectedLanguages { get; init; }
}