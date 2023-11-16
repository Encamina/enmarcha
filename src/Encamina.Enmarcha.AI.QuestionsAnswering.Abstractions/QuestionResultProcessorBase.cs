using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Base class for question results handlers processor.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class QuestionResultProcessorBase : OrderableHandlerManagerBase<IQuestionResultHandler>, IQuestionResultProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionResultProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of question result handlers for this processor.</param>
    protected QuestionResultProcessorBase(IEnumerable<IQuestionResultHandler> handlers) : base(handlers)
    {
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<IAnswer>> ProcessAsync(IQuestionResult questionResult, CancellationToken cancellationToken)
    {
        if (Handlers?.Any() ?? false)
        {
            var answers = new List<IAnswer>();

            foreach (var handler in Handlers)
            {
                answers.AddRange(await handler.HandleAsync(questionResult, cancellationToken));
            }

            return answers.AsReadOnly();
        }

        return questionResult.Answers;
    }
}
