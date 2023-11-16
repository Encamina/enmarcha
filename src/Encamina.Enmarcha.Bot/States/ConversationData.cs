using System.Text;

using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.States;

/// <summary>
/// Represents conversartion data.
/// </summary>
public class ConversationData
{
    /// <summary>
    /// Gets the conversation log.
    /// </summary>
    public List<Activity> ConversationLog { get; } = new List<Activity>();

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        foreach (var activity in ConversationLog)
        {
            if (!string.IsNullOrEmpty(activity.Text))
            {
                stringBuilder.AppendLine($"{activity.Timestamp:yyyy-MM-dd HH:mm} UTC ({activity.Recipient.Id}) From: '{activity.From.Name}' To: '{activity.Recipient.Name}' | {activity.Text}\n");
            }
        }

        return stringBuilder.ToString();
    }
}
