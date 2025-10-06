using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Models;
using Encamina.Enmarcha.Agents.Telemetry;

using Microsoft.Agents.Builder;
using Microsoft.Extensions.Logging;

using IMiddleware = Microsoft.Agents.Builder.IMiddleware;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Middleware for storing incoming activity to make it available to the <see cref="TelemetryAgentIdInitializer"/>.
/// </summary>
public class TelemetryInitializerMiddleware : IMiddleware
{
    private readonly TelemetryLoggerMiddleware telemetryLoggerMiddleware;
    private readonly ILogger<TelemetryInitializerMiddleware> logger;
    private readonly bool logActivityTelemetry;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryInitializerMiddleware"/> class.
    /// </summary>
    /// <param name="telemetryLoggerMiddleware">The TelemetryLoggerMiddleware to allow for logging of activity events.</param>
    /// <param name="logger">The logger to use to log information.</param>
    /// <param name="logActivityTelemetry">Indicates if the TelemetryLoggerMiddleware should be executed to log activity events.</param>
    public TelemetryInitializerMiddleware(TelemetryLoggerMiddleware telemetryLoggerMiddleware, ILogger<TelemetryInitializerMiddleware> logger, bool logActivityTelemetry = true)
    {
        this.telemetryLoggerMiddleware = telemetryLoggerMiddleware;
        this.logger = logger;
        this.logActivityTelemetry = logActivityTelemetry;
    }

    /// <summary>
    /// Stores relevant information from the incoming activity in the <see cref="TelemetryAgentIdInitializer"/>.
    /// </summary>
    /// <param name="context">The context object for this turn.</param>
    /// <param name="nextTurn">The delegate to call to continue the agent middleware pipeline.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    /// <seealso cref="ITurnContext"/>
    /// <seealso cref="Microsoft.Agents.Core.Models.IActivity"/>
    public async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(context);

        if (context.Activity != null)
        {
            // Create and set context for this async flow
            var telemetryContext = AgentTelemetryContext.FromActivity(context.Activity);
            TelemetryAgentIdInitializer.SetCurrentContext(telemetryContext);

            logger.LogDebug("Telemetry context set for RequestId: {RequestId}", context.Activity.RequestId);

            try
            {
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
            finally
            {
                // Clear context after turn completion
                TelemetryAgentIdInitializer.ClearCurrentContext();
                logger.LogDebug("Telemetry context cleared for RequestId: {RequestId}", context.Activity.RequestId);
            }
        }
        else
        {
            logger.LogWarning("Activity is null, telemetry context not set");

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
}