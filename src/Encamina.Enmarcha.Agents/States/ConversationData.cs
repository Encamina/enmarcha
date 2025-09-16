using System.Text;

using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.States;

/// <summary>
/// Represents conversation data.
/// </summary>
public class ConversationData
{
    /// <summary>
    /// Gets the conversation log.
    /// </summary>
    public List<Activity> ConversationLog { get; } = [];

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        foreach (var activity in ConversationLog.Where(activity => !string.IsNullOrEmpty(activity.Text)))
        {
            stringBuilder.AppendLine($"{activity.Timestamp:yyyy-MM-dd HH:mm} UTC ({activity.Recipient.Id}) From: '{activity.From.Name}' To: '{activity.Recipient.Name}' | {activity.Text}\n");
        }

        return stringBuilder.ToString();
    }
}
