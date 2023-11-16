using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a handler for question requests.
/// </summary>
public interface IQuestionRequestHandler : IOrderable
{
    /// <summary>
    /// Handles question requets by updating the given <paramref name="questionRequest"/> instance.
    /// </summary>
    /// <typeparam name="TQuestionRequest">The type for the specific question request to handle.</typeparam>
    /// <param name="questionRequest">A valid question request instance.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An instance of <typeparamref name="TQuestionRequest"/> with values from the handled metadata.</returns>
    Task<TQuestionRequest> HandleAsync<TQuestionRequest>(TQuestionRequest questionRequest, CancellationToken cancellationToken)
        where TQuestionRequest : IQuestionRequest;
}
