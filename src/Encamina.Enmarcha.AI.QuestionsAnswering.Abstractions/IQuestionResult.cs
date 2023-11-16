namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents the result of a question request.
/// </summary>
public interface IQuestionResult
{
    /// <summary>
    /// Gets the collection of answers on this result.
    /// </summary>
    IReadOnlyList<IAnswer> Answers { get; init; }
}
