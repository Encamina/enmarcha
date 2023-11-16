using System.Globalization;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Options for intent predictions.
/// </summary>
public class IntentPredictionOptions
{
    /// <summary>
    /// Gets a language to use when predicting intents.
    /// </summary>
    public virtual CultureInfo Language { get; init; }
}
