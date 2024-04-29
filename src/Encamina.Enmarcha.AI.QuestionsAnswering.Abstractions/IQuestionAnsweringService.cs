using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a cognitive service that provides question answering capabilities.
/// </summary>
public interface IQuestionAnsweringService : ICognitiveService
{
    /// <summary>
    /// Obtains answers from a given question request.
    /// </summary>
    /// <param name="request">A question request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A question result with found answers.</returns>
    Task<QuestionResult> GetAnswersAsync(QuestionRequest request, CancellationToken cancellationToken);
}
