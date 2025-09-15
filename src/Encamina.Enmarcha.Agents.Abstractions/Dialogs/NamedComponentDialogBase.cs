using Encamina.Enmarcha.Agents.Abstractions.Extensions;

using Microsoft.Agents.Builder.Dialogs;

namespace Encamina.Enmarcha.Agents.Abstractions.Dialogs;

/// <summary>
/// Base class for an agent's dialog composed of other dialogs.
/// </summary>
/// <seealso cref="ComponentDialog"/>
public class NamedComponentDialogBase : ComponentDialog, INameableDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NamedComponentDialogBase"/> class.
    /// </summary>
    protected NamedComponentDialogBase() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NamedComponentDialogBase"/> class.
    /// </summary>
    /// <param name="dialogId">The unique identifier for this dialog.</param>
    protected NamedComponentDialogBase(string? dialogId) : base(dialogId)
    {
    }

    /// <summary>
    /// Gets the name of this agent's dialog.
    /// </summary>
    public virtual string Name => this.RemoveDialogSuffix(GetType().Name);

    /// <inheritdoc/>
    public string Suffix => @"Dialog";
}
