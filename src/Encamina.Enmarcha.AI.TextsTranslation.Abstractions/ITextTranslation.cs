using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents the value of a transalations text.
/// </summary>
public interface ITextTranslation : IIdentifiableValuable<string, string>, IConfidenceScore
{
    /// <summary>
    /// Gets text translations per language.
    /// This dictionary expects a language code as key, and a translated text as value.
    /// </summary>
    IDictionary<string, string> Translations { get; }
}