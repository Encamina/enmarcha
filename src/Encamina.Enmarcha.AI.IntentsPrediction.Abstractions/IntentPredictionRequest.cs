using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// A request for an intent prediction from an utterance.
/// </summary>
public class IntentPredictionRequest : IIntentPredictionRequest
{
    /// <inheritdoc/>
    public string Id { get; init; } = Guid.NewGuid().ToString();

    /// <inheritdoc/>
    public string ParticipantId { get; init; } = Guid.NewGuid().ToString();

    /// <inheritdoc/>
    public string Utterance { get; init; }

    /// <inheritdoc/>
    public IntentPredictionOptions IntentPredictionOptions { get; init; }

    /// <inheritdoc/>
    object IIdentifiable.Id => Id;
}
