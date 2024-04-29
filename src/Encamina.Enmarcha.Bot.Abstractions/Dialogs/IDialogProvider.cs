using Microsoft.Bot.Builder.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Represents a dialog provider to get <see cref="Dialog"/>s by its unique identifier.
/// </summary>
public interface IDialogProvider
{
    /// <summary>
    /// Gets a <see cref="Dialog"/> from its unique identifier.
    /// </summary>
    /// <param name="dialogId">The unique identifier of the <see cref="Dialog"/> to get.</param>
    /// <returns>
    /// A valid <see cref="Dialog"/>.
    /// </returns>
    Dialog? GetById(string dialogId);

    /// <summary>
    /// Tries to get a <see cref="Dialog"/> from its unique identifier.
    /// </summary>
    /// <param name="dialogId">The unique identifier of the <see cref="Dialog"/> to get.</param>
    /// <param name="dialog">
    /// When this method returns, the <see cref="Dialog"/> associated with the specified unique identifier, if
    /// the unique identifier is found; otherwise, <see langword="null"/> or default's <see cref="Dialog"/> value.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a valid <see cref="Dialog"/> is found by its unique identifier; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetById(string dialogId, out Dialog? dialog);
}
