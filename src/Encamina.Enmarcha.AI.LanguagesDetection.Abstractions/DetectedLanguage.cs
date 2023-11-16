using System.Globalization;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <inheritdoc/>
public class DetectedLanguage : IdentifiableBase<string>, IDetectedLanguage
{
    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }

    /// <inheritdoc/>
    public virtual CultureInfo Language { get; init; }
}