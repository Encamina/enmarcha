using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a handler for question results.
/// </summary>
public interface IQuestionResultHandler : IOrderable
{
    /// <summary>
    /// Handles question results by retrieving answers a the given <paramref name="questionResult"/> instance.
    /// </summary>
    /// <param name="questionResult">A valid question result instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A collection of answers from handling the given query result.</returns>
    Task<IEnumerable<IAnswer>> HandleAsync(IQuestionResult questionResult, CancellationToken cancellationToken);
}
