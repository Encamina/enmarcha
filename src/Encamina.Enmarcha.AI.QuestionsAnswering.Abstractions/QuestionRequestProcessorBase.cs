using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Base class for question requests handlers processor.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class QuestionRequestProcessorBase : OrderableHandlerManagerBase<IQuestionRequestHandler>, IQuestionRequestProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionRequestProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of question request handlers for this processor.</param>
    protected QuestionRequestProcessorBase(IEnumerable<IQuestionRequestHandler> handlers) : base(handlers)
    {
    }

    /// <inheritdoc/>
    public virtual async Task<TQuestionRequest> ProcessAsync<TQuestionRequest>(string question, string userId, IQuestionRequestOptions options, CancellationToken cancellationToken)
        where TQuestionRequest : IQuestionRequest, new()
    {
        Guard.IsNotNullOrWhiteSpace(question);

        var questionRequest = new TQuestionRequest()
        {
            Question = question,
            Options = options,
            UserId = userId,
        };

        if (Handlers?.Any() ?? false)
        {
            foreach (var handler in Handlers)
            {
                questionRequest = await handler.HandleAsync(questionRequest, cancellationToken);
            }
        }

        return questionRequest;
    }
}
