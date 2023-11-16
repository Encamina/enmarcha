using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Represents normalizer that fixes or changes results from text translations to overcome or correct any discrepancy
/// or unexpected result.
/// </summary>
public interface ITextTranslationNormalizer : IOrderable
{
    /// <summary>
    /// Normalizes the given <see cref="string"/> value.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>A normalize <see cref="string"/> value.</returns>
    string Normalize(string value);
}
