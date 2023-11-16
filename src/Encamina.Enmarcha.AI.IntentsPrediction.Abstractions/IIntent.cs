using Encamina.Enmarcha.AI.Abstractions;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

/// <summary>
/// An intent, usually inferred from an utterance.
/// </summary>
public interface IIntent : INameable, IConfidenceScore
{
}
