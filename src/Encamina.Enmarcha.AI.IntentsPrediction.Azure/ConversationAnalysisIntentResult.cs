using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// An intent result that corresponds to a conversation analysis intent.
/// </summary>
public class ConversationAnalysisIntentResult : IIntentResult
{
    /// <inheritdoc/>
    public virtual IntentKindBase Kind => IntentKind.ConversationAnalysis;

    /// <inheritdoc/>
    public virtual string Name { get; init; }

    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }

    /// <summary>
    /// Gets a collection of entities obtained or extracted from the intent.
    /// </summary>
    public IReadOnlyList<ConversationAnalysisEntity> ConversationEntities { get; init; } = Array.Empty<ConversationAnalysisEntity>();

    /// <summary>
    /// Gets a collection of specific conversation analysis intents.
    /// </summary>
    public IReadOnlyList<ConversationAnalysisIntent> ConversationIntents { get; init; } = Array.Empty<ConversationAnalysisIntent>();
}
