using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// An intent result that corresponds to a question answering intent.
/// </summary>
public class QuestionAnsweringIntentResult : IIntentResult
{
    /// <inheritdoc/>
    public virtual IntentKindBase Kind => IntentKind.QuestionAnswering;

    /// <inheritdoc/>
    public virtual string Name { get; init; }

    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }

    /// <summary>
    /// Gets the answers obtained from the intent prediction.
    /// </summary>
    public IReadOnlyList<Answer> Answers { get; init; } = Array.Empty<Answer>();
}
