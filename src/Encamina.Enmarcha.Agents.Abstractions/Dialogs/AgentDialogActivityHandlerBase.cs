using System.Diagnostics.CodeAnalysis;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.Compat;
using Microsoft.Agents.Builder.State;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Abstractions.Dialogs;

/// <summary>
/// An abstract implementation of <see cref="IAgent "/> derived from <see cref="ActivityHandler"/>.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AgentDialogActivityHandlerBase : ActivityHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentDialogActivityHandlerBase"/> class.
    /// </summary>
    /// <param name="conversationState">The conversation state for this agent.</param>
    /// <param name="userState">The user state for this agent.</param>
    /// <param name="logger">The logger for this agent.</param>
    protected AgentDialogActivityHandlerBase(ConversationState conversationState, UserState userState, ILogger<AgentDialogActivityHandlerBase> logger)
    {
        ConversationState = conversationState;
        UserState = userState;
        Logger = logger;
    }

    /// <summary>
    /// Gets the conversation state of this agent.
    /// </summary>
    public virtual AgentState ConversationState { get; init; }

    /// <summary>
    /// Gets the user state of this agent.
    /// </summary>
    public virtual AgentState UserState { get; init; }

    /// <summary>
    /// Gets the logger of this agent.
    /// </summary>
    public virtual ILogger Logger { get; init; }

    /// <inheritdoc/>
    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
    {
        await base.OnTurnAsync(turnContext, cancellationToken);

        // Save any state changes that might have occurred during the turn.
        await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
    }
}
