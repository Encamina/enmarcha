using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.Compat;
using Microsoft.Agents.Builder.Dialogs;
using Microsoft.Agents.Builder.State;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Abstractions.Dialogs;

/// <summary>
/// <para>
/// An abstract implementation of <see cref="IAgent "/> derived from <see cref="AgentDialogActivityHandlerBase"/> and <see cref="ActivityHandler"/> to run any type
/// of <see cref="Dialog"/>. The use of type parameterization is to allows multiple different agents to be run at different endpoints within the
/// same project. This can be achieved by defining distinct <c>Controller</c> types each with dependency on distinct <see cref="IAgent"/> types,
/// allowing Dependency Injection to glue everything together without ambiguity.
/// </para>
/// <para>
/// The <see cref="ConversationState"/> is used by the <see cref="Dialog"/> system. The <see cref="UserState"/> isn't, however,
/// it might have been used in a <see cref="Dialog"/> implementation, and the requirement is that all <see cref="AgentState"/> objects
/// are saved at the end of a turn.
/// </para>
/// </summary>
/// <typeparam name="TRootDialog">A valid <see cref="Dialog"/> type that represents the root (or main) dialog for this agent.</typeparam>
public class AgentDialogActivityHandlerBase<TRootDialog> : AgentDialogActivityHandlerBase
    where TRootDialog : Dialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AgentDialogActivityHandlerBase{TRootDialog}"/> class.
    /// </summary>
    /// <param name="rootDialog">The root (or main) <see cref="Dialog"/> for this agent.</param>
    /// <param name="conversationState">The conversation state for this agent.</param>
    /// <param name="userState">The user state for this agent.</param>
    /// <param name="logger">The logger for this agent.</param>
    protected AgentDialogActivityHandlerBase(TRootDialog rootDialog, ConversationState conversationState, UserState userState, ILogger<AgentDialogActivityHandlerBase<TRootDialog>> logger)
        : base(conversationState, userState, logger)
    {
        Dialog = rootDialog;
    }

    /// <summary>
    /// Gets the main dialog of this agent.
    /// </summary>
    public virtual TRootDialog Dialog { get; init; }
}
