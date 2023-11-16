using System.Globalization;

using Encamina.Enmarcha.AI.Abstractions;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <summary>
/// Represents the detected language from a text.
/// </summary>
public interface IDetectedLanguage : IIdentifiable<string>, IConfidenceScore
{
    /// <summary>
    /// Gets a <see cref="CultureInfo"/> that represents the detected language.
    /// </summary>
    public CultureInfo Language { get; init; }
}