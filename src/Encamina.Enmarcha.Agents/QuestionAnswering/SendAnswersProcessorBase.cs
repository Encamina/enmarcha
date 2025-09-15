using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Agents.Abstractions.QuestionAnswering;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Agents.Builder;

namespace Encamina.Enmarcha.Agents.QuestionAnswering;

/// <summary>
/// Base class for answer handlers processor.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class SendAnswersProcessorBase : OrderableHandlerManagerBase<ISendAnswersHandler>, ISendAnswersProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendAnswersProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of answer handlers for this processor.</param>
    protected SendAnswersProcessorBase(IEnumerable<ISendAnswersHandler> handlers) : base(handlers)
    {
    }

    /// <summary>
    /// Sends a response by processing and handling a given collection of answers.
    /// </summary>
    /// <typeparam name="TAnswer">The type of answers.</typeparam>
    /// <param name="context">The context object for this turn of the agent to use when sending the response.</param>
    /// <param name="answers">A collection of answers to process and handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> indicating
    /// that the process has succeeded and no further processing is required. Otherwise, the <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    public virtual Task<SendResponseResult> SendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken)
        where TAnswer : IAnswer => SendResponseAsync(context, answers, false, cancellationToken);

    /// <summary>
    /// Sends a response by processing and handling a given collection of answers, with optional verbose information.
    /// </summary>
    /// <typeparam name="TAnswer">The type of answers.</typeparam>
    /// <param name="context">The context object for this turn of the agent to use when sending the response.</param>
    /// <param name="answers">A collection of answers to process and handle.</param>
    /// <param name="withVerbose">Indicates whether verbose information should be included in the response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> indicating
    /// that the process has succeeded and no further processing is required. Otherwise, the <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    public virtual async Task<SendResponseResult> SendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, bool withVerbose, CancellationToken cancellationToken)
        where TAnswer : IAnswer
    {
        if (context != null && (answers?.Any() ?? false) && (Handlers?.Any() ?? false))
        {
            foreach (var handler in Handlers)
            {
                var result = await handler.HandleSendResponseAsync(context, answers, withVerbose, cancellationToken);

                if (result.Successful)
                {
                    return result;
                }
            }
        }

        return SendResponseResult.Empty;
    }
}
