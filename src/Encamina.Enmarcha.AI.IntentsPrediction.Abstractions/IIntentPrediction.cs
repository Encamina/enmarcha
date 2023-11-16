namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Represents a prediction of an intent.
/// </summary>
public interface IIntentPrediction
{
    /// <summary>
    /// Gets the name of the intent with the highest score.
    /// </summary>
    string TopIntentName { get; }

    /// <summary>
    /// Gets a dictionary that contains all intents. The key is the intent's name, and the value is a valid
    /// instance of <see cref="IIntentResult"/>. The top intent's value (property '<see cref="TopIntentName"/>')
    /// also contains the actual response from the target project.
    /// </summary>
    IReadOnlyDictionary<string, IIntentResult> Intents { get; }
}
