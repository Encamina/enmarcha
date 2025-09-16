using System.Text.Json;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Telemetry;

using Microsoft.Agents.Builder;
using Microsoft.AspNetCore.Http;

using IMiddleware = Microsoft.Agents.Builder.IMiddleware;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Middleware for storing incoming activity on the HttpContext to make it available to the <see cref="TelemetryAgentIdInitializer"/>.
/// </summary>
public class TelemetryInitializerMiddleware : IMiddleware
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TelemetryLoggerMiddleware telemetryLoggerMiddleware;
    private readonly bool logActivityTelemetry;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryInitializerMiddleware"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The IHttpContextAccessor to allow access to the HttpContext.</param>
    /// <param name="telemetryLoggerMiddleware">The TelemetryLoggerMiddleware to allow for logging of activity events.</param>
    /// <param name="logActivityTelemetry">Indicates if the TelemetryLoggerMiddleware should be executed to log activity events.</param>
    public TelemetryInitializerMiddleware(IHttpContextAccessor httpContextAccessor, TelemetryLoggerMiddleware telemetryLoggerMiddleware, bool logActivityTelemetry = true)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        this.telemetryLoggerMiddleware = telemetryLoggerMiddleware;
        this.logActivityTelemetry = logActivityTelemetry;
    }

    /// <summary>
    /// Stores the incoming activity as JSON in the items collection on the HttpContext.
    /// </summary>
    /// <param name="context">The context object for this turn.</param>
    /// <param name="nextTurn">The delegate to call to continue the agent middleware pipeline.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    /// <seealso cref="ITurnContext"/>
    /// <seealso cref="Microsoft.Agents.Core.Models.IActivity"/>
    public virtual async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(context);

        if (context.Activity != null)
        {
            var activity = context.Activity;

            var httpContext = httpContextAccessor.HttpContext;
            var items = httpContext?.Items;

            var activityJson = JsonSerializer.SerializeToNode(activity);

            items?.Remove(TelemetryAgentIdInitializer.AgentActivityKey);

            items?.Add(TelemetryAgentIdInitializer.AgentActivityKey, activityJson);
        }

        if (logActivityTelemetry)
        {
            await telemetryLoggerMiddleware
                .OnTurnAsync(context, nextTurn, cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            await nextTurn(cancellationToken).ConfigureAwait(false);
        }
    }
}