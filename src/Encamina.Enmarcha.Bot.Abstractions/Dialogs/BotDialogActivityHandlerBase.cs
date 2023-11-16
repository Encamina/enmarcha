using System.Diagnostics.CodeAnalysis;

using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// An abstract implementation of <see cref="IBot "/> derived from <see cref="ActivityHandler"/>.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class BotDialogActivityHandlerBase : ActivityHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotDialogActivityHandlerBase"/> class.
    /// </summary>
    /// <param name="conversationState">The conversaton state for this bot.</param>
    /// <param name="userState">The user state for this bot.</param>
    /// <param name="logger">The logger for this bot.</param>
    protected BotDialogActivityHandlerBase(ConversationState conversationState, UserState userState, ILogger<BotDialogActivityHandlerBase> logger)
    {
        ConversationState = conversationState;
        UserState = userState;
        Logger = logger;
    }

    /// <summary>
    /// Gets the conversation state of this bot.
    /// </summary>
    public virtual BotState ConversationState { get; init; }

    /// <summary>
    /// Gets the user state of this bot.
    /// </summary>
    public virtual BotState UserState { get; init; }

    /// <summary>
    /// Gets the logger of this bot.
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
