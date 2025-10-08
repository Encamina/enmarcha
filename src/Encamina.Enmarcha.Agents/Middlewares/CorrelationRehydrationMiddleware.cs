using System.Diagnostics;

using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

using Microsoft.Agents.Builder;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Middleware that restores distributed correlation information for incoming bot activities.
/// </summary>
/// <remarks>
/// Retrieves a <see cref="CorrelationEntry"/> from an <see cref="ICorrelationStore"/>
/// using the incoming activity’s conversation and activity IDs.
/// If correlation data is found, it parses the stored W3C <c>traceparent</c> and <c>tracestate</c>,
/// builds an <see cref="ActivityContext"/>, and starts a <see cref="Activity"/> using the internal <see cref="ActivitySource"/>.
/// If no correlation data is available, it starts a fallback internal activity instead.
/// The activity is automatically stopped when the turn completes.
///
/// <para>
/// The <see cref="ActivitySource"/> defined in <see cref="TelemetryConstants.BotActivitySource"/> 
/// must be registered in the application to enable full trace correlation.
/// </para>
/// </remarks>
public sealed class CorrelationRehydrationMiddleware : IMiddleware
{
    private static readonly ActivitySource Source = new(TelemetryConstants.BotActivitySource);
    private readonly ICorrelationStore store;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationRehydrationMiddleware"/> class.
    /// </summary>
    /// <param name="store">The correlation store used to retrieve stored correlation entries.</param>
    public CorrelationRehydrationMiddleware(ICorrelationStore store)
    {
        this.store = store;
    }

    /// <inheritdoc />
    public async Task OnTurnAsync(ITurnContext context, NextDelegate next, CancellationToken cancellationToken)
    {
        Activity? activity = null;

        var convId = context.Activity?.Conversation?.Id;
        var actId = context.Activity?.Id;

        try
        {
            if (!string.IsNullOrEmpty(convId) && !string.IsNullOrEmpty(actId))
            {
                var corr = await store.GetAsync(convId!, actId!, cancellationToken);
                if (corr is not null && !string.IsNullOrWhiteSpace(corr.TraceParent))
                {
                    var parent = ActivityContext.Parse(corr.TraceParent, corr.TraceState);
                    activity = Source.StartActivity("OnTurnAsync", ActivityKind.Consumer, parent);

                    activity?.AddBaggage(TelemetryConstants.ConversationIdProperty, convId!);
                    activity?.AddBaggage(TelemetryConstants.ActivityIdProperty, actId!);

                    activity?.SetTag(TelemetryConstants.ConversationIdProperty, convId);
                    activity?.SetTag(TelemetryConstants.ActivityIdProperty, actId);
                    activity?.SetTag(TelemetryConstants.ChannelIdProperty, context.Activity?.ChannelId);
                    activity?.SetTag(TelemetryConstants.ActivityTypeProperty, context.Activity?.Type);
                    activity?.SetTag(TelemetryConstants.UserIdProperty, context.Activity?.From?.Id);
                }
            }

            await next(cancellationToken);
        }
        finally
        {
            activity?.Stop();
        }
    }
}
