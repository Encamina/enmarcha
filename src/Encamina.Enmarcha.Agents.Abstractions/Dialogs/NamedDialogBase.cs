using Encamina.Enmarcha.Agents.Abstractions.Extensions;

using Microsoft.Agents.Builder.Dialogs;

namespace Encamina.Enmarcha.Agents.Abstractions.Dialogs;

/// <summary>
/// Base class for simple agent's dialogs.
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
    protected NamedDialogBase(string? dialogId) : base(dialogId)
    {
    }

    /// <summary>
    /// Gets the name of this agent's dialog.
    /// </summary>
    public virtual string Name => this.RemoveDialogSuffix(GetType().Name);

    /// <inheritdoc/>
    public string Suffix => @"Dialog";
}
