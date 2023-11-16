namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// A result from an intent prediction request.
/// </summary>
public class IntentPredictionResult : IIntentPredictionResult
{
    /// <inheritdoc/>
    public virtual IntentPrediction IntentPrediction { get; init; }
}
