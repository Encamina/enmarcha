using System.Globalization;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// A request for text translations.
/// </summary>
public class TextTranslationRequest : ITextTranslationRequest
{
    /// <inheritdoc/>
    public virtual CultureInfo? FromLanguage { get; init; }

    /// <inheritdoc/>
    public virtual ICollection<CultureInfo> ToLanguages { get; init; } = new List<CultureInfo>();

    /// <inheritdoc/>
    public virtual IDictionary<string, string> Texts { get; init; } = new Dictionary<string, string>();
}