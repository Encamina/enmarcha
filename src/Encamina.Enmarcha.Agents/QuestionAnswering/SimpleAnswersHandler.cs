using System.Text.Json;

using Encamina.Enmarcha.Agents.Abstractions.QuestionAnswering;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.QuestionAnswering;

/// <summary>
/// A simple answer handler that sends the first answer found with the highest confidence score as the response.
/// </summary>
public class SimpleAnswersHandler : ISendAnswersHandler
{
    /// <inheritdoc/>
    public virtual int Order => int.MaxValue; // Ensure this is the last response handler to be evaluated.

    /// <inheritdoc/>
    public virtual Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken)
        where TAnswer : IAnswer => HandleSendResponseAsync(context, answers, false, cancellationToken);

    /// <inheritdoc/>
    public virtual async Task<SendResponseResult> HandleSendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, bool withVerbose, CancellationToken cancellationToken)
        where TAnswer : IAnswer
    {
        if (context != null && (answers?.Any() ?? false))
        {
            var highestConfidenceScoredAnswer = answers.OrderByDescending(a => a.ConfidenceScore).First().Value;
            var activity = MessageFactory.Text(highestConfidenceScoredAnswer, highestConfidenceScoredAnswer);

            if (withVerbose)
            {
                activity.Properties = BuildVerboseInformation(answers);
            }

            return new SendResponseResult(await context.SendActivityAsync(activity, cancellationToken: cancellationToken));
        }

        return SendResponseResult.Empty;
    }

    /// <summary>
    /// Builds verbose information to add into an activity.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the answers.</typeparam>
    /// <param name="answers">A collection of answers that can be used to build verbose information.</param>
    /// <returns>
    /// A dictionary with a single entry where the key is "Verbose" and the value is a <see cref="JsonElement"/> containing the serialized answers.
    /// </returns>
    protected virtual IDictionary<string, JsonElement> BuildVerboseInformation<TAnswer>(IEnumerable<TAnswer> answers) where TAnswer : IAnswer
    {
        return new Dictionary<string, JsonElement> { ["Verbose"] = JsonSerializer.SerializeToElement(new { Answers = answers }) };
    }
}
