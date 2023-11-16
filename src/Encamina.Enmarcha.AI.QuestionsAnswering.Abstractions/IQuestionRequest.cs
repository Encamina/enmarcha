namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a request for a question.
/// </summary>
public interface IQuestionRequest
{
    /// <summary>
    /// Gets the question in the request.
    /// </summary>
    string Question { get; init; }

    /// <summary>
    /// Gets the options for this request.
    /// </summary>
    IQuestionRequestOptions Options { get; init; }

    /// <summary>
    /// Gets a unique identifier for the user asking the question.
    /// </summary>
    string UserId { get; init; }
}