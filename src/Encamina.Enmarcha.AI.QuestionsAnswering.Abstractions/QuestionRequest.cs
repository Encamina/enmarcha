namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// A request for a question.
/// </summary>
public class QuestionRequest : IQuestionRequest
{
    /// <inheritdoc/>
    public virtual string Question { get; init; }

    /// <inheritdoc/>
    public virtual IQuestionRequestOptions Options { get; init; }

    /// <inheritdoc/>
    public virtual string UserId { get; init; }
}
