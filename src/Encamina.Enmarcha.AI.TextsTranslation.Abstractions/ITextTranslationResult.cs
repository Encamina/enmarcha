namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents the result value for translation text.
/// </summary>
public interface ITextTranslationResult
{
    /// <summary>
    /// Gets the text translations.
    /// </summary>
    IEnumerable<TextTranslation> TextTranslations { get; }
}
