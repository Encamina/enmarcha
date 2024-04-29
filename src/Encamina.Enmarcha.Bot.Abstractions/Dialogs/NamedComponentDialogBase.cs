using Encamina.Enmarcha.Bot.Abstractions.Extensions;

using Microsoft.Bot.Builder.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Base class for a bot's dialog composed of other dialogs.
/// </summary>
/// <seealso cref="ComponentDialog"/>
public class NamedComponentDialogBase : ComponentDialog, INameableDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedComponentDialogBase"/> class.
    /// </summary>
    protected NamedComponentDialogBase() : this(null!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedComponentDialogBase"/> class.
    /// </summary>
    /// <param name="dialogId">The unique identifier for this dialog.</param>
    protected NamedComponentDialogBase(string dialogId) : base(dialogId)
    {
    }

    /// <summary>
    /// Gets the name of this bot's dialog.
    /// </summary>
    public virtual string Name => this.RemoveDialogSufix(GetType().Name);

    /// <inheritdoc/>
    public string Suffix => @"Dialog";
}
