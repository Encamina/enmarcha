using System.Text.Json;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// Initializer that sets the user ID and session ID (in addition to other agent-specific properties such as activity ID).
/// </summary>
public class TelemetryAgentIdInitializer : ITelemetryInitializer
{
    /// <summary>
    /// Constant key used for storing activity information in turn state.
    /// </summary>
    public const string AgentActivityKey = "AgentActivity";

    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryAgentIdInitializer"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HttpContextAccessor used to access the current HttpContext.</param>
    public TelemetryAgentIdInitializer(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc/>
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry == null)
        {
            return;
        }

        var httpContext = httpContextAccessor.HttpContext;
        var items = httpContext?.Items;

        if (items != null)
        {
            if (telemetry is RequestTelemetry or EventTelemetry or TraceTelemetry or DependencyTelemetry or PageViewTelemetry
                && items.TryGetValue(AgentActivityKey, out var agentActivity) && agentActivity is JsonElement body)
            {
                var userId = string.Empty;
                if (body.TryGetProperty("from", out var from) && from.TryGetProperty("id", out var fromId))
                {
                    userId = fromId.GetString() ?? string.Empty;
                }

                var channelId = body.GetProperty("channelId").GetString();

                var conversationId = string.Empty;
                var sessionId = string.Empty;
                if (body.TryGetProperty("conversation", out var conversation) && conversation.TryGetProperty("id", out var convId))
                {
                    conversationId = convId.GetString() ?? string.Empty;
                    sessionId = conversationId.Hash();
                }

                // Set the user id on the Application Insights telemetry item.
                telemetry.Context.User.Id = channelId + userId;

                // Set the session id on the Application Insights telemetry item.
                // Hashed ID is used due to max session ID length for App Insights session Id
                telemetry.Context.Session.Id = sessionId;

                var telemetryProperties = ((ISupportProperties)telemetry).Properties;

                telemetryProperties.TryAdd("conversationId", conversationId);

                if (!telemetryProperties.ContainsKey("activityId"))
                {
                    telemetryProperties.Add("activityId", body.GetProperty("id").GetString());
                }

                telemetryProperties.TryAdd("channelId", channelId);

                if (!telemetryProperties.ContainsKey("activityType"))
                {
                    telemetryProperties.Add("activityType", body.GetProperty("type").GetString());
                }
            }
        }
    }
}
