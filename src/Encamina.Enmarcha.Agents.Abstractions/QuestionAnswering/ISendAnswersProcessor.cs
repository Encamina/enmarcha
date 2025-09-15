using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Microsoft.Agents.Builder;

namespace Encamina.Enmarcha.Agents.Abstractions.QuestionAnswering;

/// <summary>
/// Represents a processor to send responses from answers.
/// </summary>
public interface ISendAnswersProcessor
{
    /// <summary>
    /// Gets the current collection of available handlers for sending a response from answers.
    /// </summary>
    IEnumerable<ISendAnswersHandler> Handlers { get; init; }

    /// <summary>
    /// Sends a response from a collection of answers using <see cref="Handlers">handlers</see>.
    /// </summary>
    /// <typeparam name="TAnswer">The specific type of the answers, which must implement <see cref="IAnswer"/>.</typeparam>
    /// <param name="context">The current context for this turn of the agent.</param>
    /// <param name="answers">A collection of answers to choose from when sending a response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> indicating
    /// that the process has succeeded and no further processing is required. Otherwise, the <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    Task<SendResponseResult> SendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken) where TAnswer : IAnswer;

    /// <summary>
    /// Sends a response from a collection of answers using <see cref="Handlers">handlers</see>.
    /// </summary>
    /// <typeparam name="TAnswer">The specific type of the answers, which must implement <see cref="IAnswer"/>.</typeparam>
    /// <param name="context">The current context for this turn of the agent.</param>
    /// <param name="answers">A collection of answers to choose from when sending a response.</param>
    /// <param name="withVerbose">Indicates whether verbose information should be included in the response.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns a valid instance of <see cref="SendResponseResult"/> with its <see cref="SendResponseResult.Successful"/> property as <see langword="true"/> indicating
    /// that the process has succeeded and no further processing is required. Otherwise, the <see cref="SendResponseResult.Successful"/> property will be <see langword="false"/>.
    /// </returns>
    Task<SendResponseResult> SendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, bool withVerbose, CancellationToken cancellationToken) where TAnswer : IAnswer;
}