using Encamina.Enmarcha.Agents.Extensions;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// Telemetry initializer that adds AI request correlation data to Application Insights telemetry.
/// It extracts specific headers from the HTTP request (e.g., ActivityId, ConversationId, UserId, UserEmail)
/// and includes them as custom properties in telemetry for better traceability.
/// </summary>
public class LogAIRequestScopeTelemetryInitializer : ITelemetryInitializer
{
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogAIRequestScopeTelemetryInitializer"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor to retrieve request headers.</param>
    public LogAIRequestScopeTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Initializes the telemetry item by adding AI request correlation data as custom properties.
    /// </summary>
    /// <param name="telemetry">The telemetry item to initialize.</param>
    public void Initialize(ITelemetry telemetry)
    {
        var context = httpContextAccessor.HttpContext;

        if (context == null)
        {
            return;
        }

        if (context.GetEndpoint()?.Metadata
                .GetMetadata<Microsoft.AspNetCore.Routing.EndpointGroupNameAttribute>()?
                .EndpointGroupName != CommonConstants.LogAIRequestScopeItems.AI)
        {
            return;
        }

        if (telemetry is ISupportProperties telemetryWithProperties)
        {
            telemetryWithProperties.Properties[CommonConstants.LogAIRequestScopeItems.ActivityId] =
                context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderActivityId);

            telemetryWithProperties.Properties[CommonConstants.LogAIRequestScopeItems.ConversationId] =
                context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderConversationId);

            telemetryWithProperties.Properties[CommonConstants.LogAIRequestScopeItems.UserId] =
                context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderUserId);

            telemetryWithProperties.Properties[CommonConstants.LogAIRequestScopeItems.UserEmail] =
                context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderUserEmail);
        }
    }
}