namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <summary>
/// Represents the result value for language detection.
/// </summary>
public interface ILanguageDetectionResult
{
    /// <summary>
    /// Gets the detected languages from texts.
    /// </summary>
    IEnumerable<DetectedLanguage> DetectedLanguages { get; init; }
}