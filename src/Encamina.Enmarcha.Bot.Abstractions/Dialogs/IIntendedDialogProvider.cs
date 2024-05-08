using Microsoft.Bot.Builder.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Dialogs;

/// <summary>
/// Represents a dialog provider to get <see cref="Dialog"/>s by its intent.
/// </summary>
public interface IIntendedDialogProvider
{
    /// <summary>
    /// Gets a <see cref="Dialog"/> from its intent.
    /// </summary>
    /// <param name="dialogIntent">The intent of the <see cref="Dialog"/> to get.</param>
    /// <returns>
    /// A valid <see cref="Dialog"/>.
    /// </returns>
    Dialog? GetByIntent(string dialogIntent);

    /// <summary>
    /// Tries to get a <see cref="Dialog"/> from its intent.
    /// </summary>
    /// <param name="dialogIntent">The intent of the <see cref="Dialog"/> to get.</param>
    /// <param name="dialog">
    /// When this method returns, the <see cref="Dialog"/> associated with the specified intent, if the
    /// intent is found; otherwise, <see langword="null"/> or default's <see cref="Dialog"/> value. This
    /// parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a valid <see cref="Dialog"/> is found by its intent; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetByIntent(string dialogIntent, out Dialog? dialog);
}
