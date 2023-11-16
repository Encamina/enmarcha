using Encamina.Enmarcha.Bot.Abstractions.Dialogs;

namespace Encamina.Enmarcha.Bot.Abstractions.Extensions;

/// <summary>
/// Extension helper methods for <see cref="INameableDialog"/>.
/// </summary>
public static class IBotNameableDialogExtensions
{
    /// <summary>
    /// Removes the <see cref="INameableDialog.Suffix"/> from the name of the bot's dialog type.
    /// </summary>
    /// <param name="botDialog">The bot's dialog.</param>
    /// <param name="nameOfDialog">The name of the bot's dialog type.</param>
    /// <returns>The name of the bot's dialog type without the suffix.</returns>
    public static string RemoveDialogSufix(this INameableDialog botDialog, string nameOfDialog)
    {
        return nameOfDialog.EndsWith(botDialog.Suffix, StringComparison.OrdinalIgnoreCase)
            ? nameOfDialog[..^botDialog.Suffix.Length]
            : nameOfDialog;
    }
}
