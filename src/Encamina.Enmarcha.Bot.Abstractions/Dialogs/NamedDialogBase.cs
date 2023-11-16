using Encamina.Enmarcha.Bot.Abstractions.Extensions;

using Microsoft.Bot.Builder.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Base class for simple bot's dialogs.
/// </summary>
/// <seealso cref="Dialog"/>
public abstract class NamedDialogBase : Dialog, INameableDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedDialogBase"/> class.
    /// </summary>
    protected NamedDialogBase() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedDialogBase"/> class.
    /// </summary>
    /// <param name="dialogId">The unique identifier for this dialog.</param>
    protected NamedDialogBase(string dialogId) : base(dialogId)
    {
    }

    /// <summary>
    /// Gets the name of this bot's dialog.
    /// </summary>
    public virtual string Name => this.RemoveDialogSufix(GetType().Name);

    /// <inheritdoc/>
    public string Suffix => @"Dialog";
}
