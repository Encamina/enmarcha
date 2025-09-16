using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Agents.Builder;

namespace Encamina.Enmarcha.Agents.Abstractions.QuestionAnswering;

/// <summary>
/// Represents a handler for sending responses from answers.
/// </summary>
public interface ISendAnswersHandler : IOrderable
{
    /// <summary>
    /// Handles sending a response from a collection of answers.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the answers.</typeparam>
    /// <param name="context">The context object for this turn of the agent to use when sending the response.</param>
    /// <param name="answers">A collection of answers to process and handle.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> if an answer is successfully
    /// handled and used to send a response, otherwise <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken) where TAnswer : IAnswer;

    /// <summary>
    /// Handles sending a response from a collection of answers, with optional verbose information.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the answers.</typeparam>
    /// <param name="context">The context object for this turn of the agent to use when sending the response.</param>
    /// <param name="answers">A collection of answers to process and handle.</param>
    /// <param name="withVerbose">Indicates whether verbose information should be included in the response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> if an answer is successfully
    /// handled and used to send a response, otherwise <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, bool withVerbose, CancellationToken cancellationToken) where TAnswer : IAnswer;
}
