namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <inheritdoc/>
public class LanguageDetectionRequest : ILanguageDetectionRequest
{
    /// <inheritdoc/>
    public virtual IEnumerable<Text> Text { get; init; }

    /// <inheritdoc/>
    public virtual IDictionary<string, string> AdditionalParameters { get; init; } = new Dictionary<string, string>();
}
