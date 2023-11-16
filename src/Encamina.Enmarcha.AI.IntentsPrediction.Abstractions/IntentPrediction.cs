namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// A prediction of an intent.
/// </summary>
public class IntentPrediction : IIntentPrediction
{
    /// <inheritdoc/>
    public virtual string TopIntentName { get; init; }

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, IIntentResult> Intents { get; init; }
}
