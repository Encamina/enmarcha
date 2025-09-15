namespace Encamina.Enmarcha.Agents.Abstractions.Telemetry;

/// <summary>
/// Defines names of common events for use with a <see cref="IAgentTelemetryClient"/> object.
/// IMPORTANT:
/// Event names intentionally keep the term "Bot" (e.g., BotMessageReceived, BotMessageSend, etc.)
/// for backward compatibility. This ensures that any existing filters, dashboards, or telemetry
/// queries relying on these identifiers continue to work without breaking.
/// </summary>
public static class TelemetryLoggerConstants
{
    /// <summary>
    /// The name of the event when a new message is received from the user.
    /// </summary>
    public const string AgentMsgReceiveEvent = "BotMessageReceived";

    /// <summary>
    /// The name of the event when logged when a message is sent from the agent to the user.
    /// </summary>
    public const string AgentMsgSendEvent = "BotMessageSend";

    /// <summary>
    /// The name of the event when a message is updated by the agent.
    /// </summary>
    public const string AgentMsgUpdateEvent = "BotMessageUpdate";

    /// <summary>
    /// The name of the event when a message is deleted by the agent.
    /// </summary>
    public const string AgentMsgDeleteEvent = "BotMessageDelete";
}