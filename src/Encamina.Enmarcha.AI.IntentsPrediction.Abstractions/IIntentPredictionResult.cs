namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Represents a result from an intent prediction request.
/// </summary>
public interface IIntentPredictionResult
{
    /// <summary>
    /// Gets the intent prediction.
    /// </summary>
    IntentPrediction IntentPrediction { get; init; }
}
