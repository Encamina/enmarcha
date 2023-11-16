using System.Globalization;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents the request for text translation.
/// </summary>
public interface ITextTranslationRequest
{
    /// <summary>
    /// Gets the language to translate from.
    /// </summary>
    CultureInfo FromLanguage { get; init; }

    /// <summary>
    /// Gets the collection of languages to translate.
    /// </summary>
    ICollection<CultureInfo> ToLanguages { get; init; }

    /// <summary>
    /// Gets the collection of texts to translate. The idea is that every element to translate has an
    /// identifier (the <see cref="IDictionary{TKey, TValue}.Keys"/>) and a value (the <see cref="IDictionary{TKey, TValue}.Values"/>) to
    /// translate.
    /// </summary>
    IDictionary<string, string> Texts { get; }
}
