using Encamina.Enmarcha.AI.Abstractions;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// An entity obtainer or extracted from an intent.
/// </summary>
public class ConversationAnalysisEntity : INameableValuable<string>, IConfidenceScore
{
    /// <inheritdoc/>
    public virtual string Name { get; init;  }

    /// <inheritdoc/>
    public virtual string Value { get; init; }

    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }
}
