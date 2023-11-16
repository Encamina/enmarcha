namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a processor for question requests handlers.
/// </summary>
public interface IQuestionRequestProcessor
{
    /// <summary>
    /// Gets the collection of question request handlers.
    /// </summary>
    IEnumerable<IQuestionRequestHandler> Handlers { get; init; }

    /// <summary>
    /// Process a question (or message representing a question) to obtain a question request, and considering given question request options.
    /// </summary>
    /// <typeparam name="TQuestionRequest">The type of question request to process.</typeparam>
    /// <param name="question">The question.</param>
    /// <param name="userId">A unique identifier for the user asking the question. This value is optional and can be <see langword="null"/>.</param>
    /// <param name="options">Question request options to use with the <typeparamref name="TQuestionRequest"/>.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A valid instance of <typeparamref name="TQuestionRequest"/> using the given <paramref name="options">options</paramref> and <paramref name="question">question</paramref>.
    /// </returns>
    Task<TQuestionRequest> ProcessAsync<TQuestionRequest>(string question, string userId, IQuestionRequestOptions options, CancellationToken cancellationToken)
        where TQuestionRequest : IQuestionRequest, new()
        ;
}