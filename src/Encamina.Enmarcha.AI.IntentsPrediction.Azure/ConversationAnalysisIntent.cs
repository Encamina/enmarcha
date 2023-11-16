using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// A specific intent related to a conversation analysis.
/// </summary>
public class ConversationAnalysisIntent : IIntent
{
    /// <inheritdoc/>
    public virtual string Name { get; init; }

    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }
}
