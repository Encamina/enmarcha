namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <summary>
/// Represents the request parameters for language detection.
/// </summary>
public interface ILanguageDetectionRequest
{
    /// <summary>
    /// Gets the text for language detection.
    /// </summary>
    IEnumerable<Text> Text { get; init; }

    /// <summary>
    /// Gets a dictionary of additional parameters that could be usefull for specific language detection services.
    /// </summary>
    IDictionary<string, string> AdditionalParameters { get; init; }
}