namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// An intent result from a prediction.
/// </summary>
public interface IIntentResult : IIntent
{
    /// <summary>
    /// Gets the kind (type) if this intent result.
    /// </summary>
    IntentKindBase Kind { get; }
}
