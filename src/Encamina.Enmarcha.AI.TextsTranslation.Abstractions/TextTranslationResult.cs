namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// A result of a text translations request.
/// </summary>
public class TextTranslationResult : ITextTranslationResult
{
    /// <inheritdoc/>
    public virtual IEnumerable<TextTranslation> TextTranslations { get; init; } = Enumerable.Empty<TextTranslation>();
}