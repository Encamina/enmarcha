using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents the value of a transalations text.
/// </summary>
public class TextTranslation : IdentifiableBase<string>, ITextTranslation
{
    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }

    /// <summary>
    /// Gets the original text that hasn been translated.
    /// </summary>
    public virtual string Value { get; init; }

    /// <inheritdoc/>
    public virtual IDictionary<string, string> Translations { get; init; } = new Dictionary<string, string>();
}
