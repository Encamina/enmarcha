namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a processor for question results handlers.
/// </summary>
public interface IQuestionResultProcessor
{
    /// <summary>
    /// Gets the collection of question result handlers.
    /// </summary>
    IEnumerable<IQuestionResultHandler> Handlers { get; init; }

    /// <summary>
    /// Process a question result to obtain the answers.
    /// </summary>
    /// <param name="questionResult">A question result with answers from a question request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A collection of <see cref="IAnswer">answers</see> obtained from processing the handles on the given question result.
    /// </returns>
    Task<IEnumerable<IAnswer>> ProcessAsync(IQuestionResult questionResult, CancellationToken cancellationToken);
}