using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Represents a request for an intent prediction from an utterance.
/// </summary>
public interface IIntentPredictionRequest : IIdentifiable<string>
{
    /// <summary>
    /// Gets the unique identifier of the participant.
    /// </summary>
    string ParticipantId { get; init; }

    /// <summary>
    /// Gets the utterance to be analyzed.
    /// </summary>
    string Utterance { get; init; }

    /// <summary>
    /// Gets options for intent prediction.
    /// </summary>
    IntentPredictionOptions IntentPredictionOptions { get; init; }
}
