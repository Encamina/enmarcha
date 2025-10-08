using Encamina.Enmarcha.Agents.Abstractions.Telemetry;
using Encamina.Enmarcha.Agents.Models;
using Encamina.Enmarcha.Core.Extensions;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// Initializer that sets the user ID and session ID (in addition to other agent-specific properties such as activity ID).
/// </summary>
public class TelemetryAgentIdInitializer : ITelemetryInitializer
{
    private static readonly AsyncLocal<AgentTelemetryContext?> CurrentContext = new();

    /// <inheritdoc/>
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry == null)
        {
            return;
        }

        var context = CurrentContext.Value;
        if (context == null)
        {
            return;
        }

        if (telemetry is RequestTelemetry or EventTelemetry or TraceTelemetry or DependencyTelemetry or PageViewTelemetry)
        {
            // Set Application Insights standard properties
            telemetry.Context.User.Id = $"{context.ChannelId}{context.UserId}";
            telemetry.Context.Session.Id = context.ConversationId.Hash();

            // Set custom properties
            if (telemetry is ISupportProperties propertiesTelemetry)
            {
                var properties = propertiesTelemetry.Properties;

                properties.TryAdd(TelemetryConstants.ConversationIdProperty, context.ConversationId);
                properties.TryAdd(TelemetryConstants.ActivityIdProperty, context.ActivityId);
                properties.TryAdd(TelemetryConstants.ChannelIdProperty, context.ChannelId);
                properties.TryAdd(TelemetryConstants.ActivityTypeProperty, context.ActivityType);
                properties.TryAdd(TelemetryConstants.RequestIdProperty, context.RequestId);
            }
        }
    }

    /// <summary>
    /// Sets the current telemetry context for the async flow.
    /// </summary>
    /// <param name="context">The telemetry context to set.</param>
    internal static void SetCurrentContext(AgentTelemetryContext context)
    {
        CurrentContext.Value = context;
    }

    /// <summary>
    /// Clears the current telemetry context.
    /// </summary>
    internal static void ClearCurrentContext()
    {
        CurrentContext.Value = null;
    }
}
