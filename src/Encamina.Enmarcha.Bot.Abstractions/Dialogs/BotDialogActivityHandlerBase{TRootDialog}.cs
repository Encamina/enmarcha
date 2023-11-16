using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// <para>
/// An abstract implementation of <see cref="IBot "/> derived from <see cref="BotDialogActivityHandlerBase"/> and <see cref="ActivityHandler"/> to run any type
/// of <see cref="Dialog"/>. The use of type parameterization is to allows multiple different bots to be run at different endpoints within the
/// same project. This can be achieved by defining distinct <c>Controller</c> types each with dependency on distinct <see cref="IBot"/> types,
/// allowing Dependency Injection to glue everything together without ambiguity.
/// </para>
/// <para>
/// The <see cref="ConversationState"/> is used by the <see cref="Dialog"/> system. The <see cref="UserState"/> isn't, however,
/// it might have been used in a <see cref="Dialog"/> implementation, and the requirement is that all <see cref="BotState"/> objects
/// are saved at the end of a turn.
/// </para>
/// </summary>
/// <typeparam name="TRootDialog">A valid <see cref="Dialog"/> type that represents the root (or main) dialog for this bot.</typeparam>
public class BotDialogActivityHandlerBase<TRootDialog> : BotDialogActivityHandlerBase
    where TRootDialog : Dialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotDialogActivityHandlerBase{TRootDialog}"/> class.
    /// </summary>
    /// <param name="rootDialog">The root (or main) <see cref="Dialog"/> for this bot.</param>
    /// <param name="conversationState">The conversaton state for this bot.</param>
    /// <param name="userState">The user state for this bot.</param>
    /// <param name="logger">The logger for this bot.</param>
    protected BotDialogActivityHandlerBase(TRootDialog rootDialog, ConversationState conversationState, UserState userState, ILogger<BotDialogActivityHandlerBase<TRootDialog>> logger)
        : base(conversationState, userState, logger)
    {
        Dialog = rootDialog;
    }

    /// <summary>
    /// Gets the main dialog of this bot.
    /// </summary>
    public virtual TRootDialog Dialog { get; init; }
}
