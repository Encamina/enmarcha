using Encamina.Enmarcha.Agents.Abstractions.Dialogs;

namespace Encamina.Enmarcha.Agents.Abstractions.Extensions;

/// <summary>
/// Extension helper methods for <see cref="INameableDialog"/>.
/// </summary>
public static class IAgentNameableDialogExtensions
{
    /// <summary>
    /// Removes the <see cref="INameableDialog.Suffix"/> from the name of the agent's dialog type.
    /// </summary>
    /// <param name="agentDialog">The agent's dialog.</param>
    /// <param name="nameOfDialog">The name of the agent's dialog type.</param>
    /// <returns>The name of the agent's dialog type without the suffix.</returns>
    public static string RemoveDialogSuffix(this INameableDialog agentDialog, string nameOfDialog)
    {
        return nameOfDialog.EndsWith(agentDialog.Suffix, StringComparison.OrdinalIgnoreCase)
            ? nameOfDialog[..^agentDialog.Suffix.Length]
            : nameOfDialog;
    }
}
