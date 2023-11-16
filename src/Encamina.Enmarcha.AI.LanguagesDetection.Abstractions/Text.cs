using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;

/// <inheritdoc/>
public class Text : IdentifiableBase<string>, IText
{
    /// <inheritdoc/>
    public virtual string Value { get; init; }
}
