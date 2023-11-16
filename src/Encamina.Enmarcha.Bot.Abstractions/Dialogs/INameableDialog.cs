using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Represents a bot's dialog.
/// </summary>
public interface INameableDialog : INameable
{
    /// <summary>
    /// Gets the suffix that accompanies the name of this dialog.
    /// </summary>
    string Suffix { get; }
}
