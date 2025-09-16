using Encamina.Enmarcha.Agents.States;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Middleware to automatically save logger in conversation state.
/// </summary>
public class ConversationStateLoggerMiddleware : IMiddleware
{
    private readonly AgentState conversationState;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversationStateLoggerMiddleware"/> class.
    /// </summary>
    /// <param name="conversationState">The conversation state.</param>
    public ConversationStateLoggerMiddleware(ConversationState conversationState)
    {
        this.conversationState = conversationState;
    }

    /// <summary>
    /// Save activity of context in AgentState by conversation id.
    /// </summary>
    /// <param name="turnContext">The context object for this turn.</param>
    /// <param name="next">The delegate to call to continue the agent middleware pipeline.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
    {
        if (turnContext.Activity.Text != null)
        {
            var conversationStateAccessors = conversationState.CreateProperty<ConversationData>(turnContext.Activity.Conversation.Id);
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData(), cancellationToken).ConfigureAwait(false);

            conversationData.ConversationLog.Add((Activity)turnContext.Activity);
            await conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken).ConfigureAwait(false);

            turnContext.OnSendActivities(async (context, activities, nextSend) =>
            {
                var responses = await nextSend().ConfigureAwait(false);

                // Activities are being sent, they are not sent yet. Therefore, there is no Timestamp, but we need it.
                // Nevertheless, the agent framework may set again this property if appropriate.
                activities.ForEach(a => a.Timestamp = DateTime.UtcNow);

                conversationData.ConversationLog.AddRange(activities.Cast<Activity>());
                await conversationState.SaveChangesAsync(context, cancellationToken: cancellationToken).ConfigureAwait(false);

                return responses;
            });
        }

        await next(cancellationToken).ConfigureAwait(false);
    }
}
