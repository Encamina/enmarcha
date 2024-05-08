using Microsoft.Bot.Builder.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Represents a dialog provider to get <see cref="Dialog"/>s by its name.
/// </summary>
public interface INamedDialogProvider
{
    /// <summary>
    /// Gets a <see cref="Dialog"/> from its name.
    /// </summary>
    /// <param name="dialogName">The name of the <see cref="Dialog"/> to get.</param>
    /// <returns>
    /// A valid <see cref="Dialog"/>.
    /// </returns>
    Dialog? GetByName(string dialogName);

    /// <summary>
    /// Tries to get a <see cref="Dialog"/> from its name.
    /// </summary>
    /// <param name="dialogName">The name of the <see cref="Dialog"/> to get.</param>
    /// <param name="dialog">
    /// When this method returns, the <see cref="Dialog"/> associated with the specified name, if the
    /// name is found; otherwise, <see langword="null"/> or default's <see cref="Dialog"/> value. This
    /// parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a valid <see cref="Dialog"/> is found by its name; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetByName(string dialogName, out Dialog? dialog);
}
