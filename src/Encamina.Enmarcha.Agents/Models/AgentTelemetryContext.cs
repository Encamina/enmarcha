using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Models;

/// <summary>
/// Represents telemetry context information extracted from an Agent activity.
/// </summary>
internal sealed class AgentTelemetryContext
{
    /// <summary>
    /// Gets or sets the unique identifier for the user who sent the activity.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conversation identifier associated with the activity.
    /// </summary>
    public string ConversationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the activity.
    /// </summary>
    public string ActivityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel identifier through which the activity arrived.
    /// </summary>
    public string ChannelId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the activity.
    /// </summary>
    public string ActivityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the request identifier correlated with the activity.
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// Creates an <see cref="AgentTelemetryContext"/> from an <see cref="IActivity"/> instance.
    /// </summary>
    /// <param name="activity">The source activity used to populate telemetry details.</param>
    /// <returns>An <see cref="AgentTelemetryContext"/> populated with activity metadata.</returns>
    public static AgentTelemetryContext FromActivity(IActivity activity)
    {
        if (activity == null)
        {
            return new AgentTelemetryContext();
        }

        var context = new AgentTelemetryContext
        {
            ActivityId = activity.Id ?? string.Empty,
            ChannelId = activity.ChannelId ?? string.Empty,
            ActivityType = activity.Type?.ToString() ?? string.Empty,
            RequestId = activity.RequestId ?? string.Empty,
        };

        if (activity.From?.Id != null)
        {
            context.UserId = activity.From.Id;
        }

        if (activity.Conversation?.Id != null)
        {
            context.ConversationId = activity.Conversation.Id;
        }

        return context;
    }
}
