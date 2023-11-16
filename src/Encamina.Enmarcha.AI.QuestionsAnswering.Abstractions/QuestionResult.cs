namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// The result of a question request.
/// </summary>
public class QuestionResult : IQuestionResult
{
    /// <inheritdoc/>
    public virtual IReadOnlyList<IAnswer> Answers { get; init; }
}
