using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// Represents a congnitive service that provides intent prediction capabilities.
/// </summary>
public interface IIntentPredictionService : ICognitiveService
{
    /// <summary>
    /// Obtains intent predictions from a given intent prediction request.
    /// </summary>
    /// <param name="request">A intent prediction request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An intent prediction result with predicted intents.</returns>
    Task<IntentPredictionResult> PredictIntentAsync(IntentPredictionRequest request, CancellationToken cancellationToken);
}
