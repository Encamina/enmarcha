using Encamina.Enmarcha.Agents.Abstractions.Adapters;
using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Hosting.AspNetCore.BackgroundQueue;
using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Agents.Adapters;

/// <summary>
/// Common options for a channel adapter.
/// </summary>
public class ChannelAdapterOptionsBase : IChannelAdapterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelAdapterOptionsBase"/> class.
    /// </summary>
    /// <param name="activityTaskQueue">An activity task queue to manage the delivery of activities.</param>
    /// <param name="channelServiceClientFactory">An environment (usually a cloud environment) used to authenticate Activity Protocol network calls.</param>
    /// <param name="agentTelemetryClient">An agent telemetry client.</param>
    /// <param name="agentStates">A collection of agent states.</param>
    /// <param name="agentMiddlewares">A collection of agent middlewares.</param>
    protected ChannelAdapterOptionsBase(IActivityTaskQueue activityTaskQueue, IChannelServiceClientFactory channelServiceClientFactory,
        IAgentTelemetryClient agentTelemetryClient, IEnumerable<AgentState> agentStates, IEnumerable<IMiddleware> agentMiddlewares)
    {
        ActivityTaskQueue = activityTaskQueue;
        ChannelServiceClientFactory = channelServiceClientFactory;
        AgentTelemetryClient = agentTelemetryClient;
        AgentStates = agentStates;
        Middlewares = agentMiddlewares;
    }

    /// <inheritdoc/>
    public virtual IActivityTaskQueue ActivityTaskQueue { get; init; }

    /// <inheritdoc/>
    public virtual IEnumerable<AgentState> AgentStates { get; init; }

    /// <inheritdoc/>
    public virtual IAgentTelemetryClient AgentTelemetryClient { get; init; }

    /// <inheritdoc/>
    public virtual IChannelServiceClientFactory ChannelServiceClientFactory { get; init; }

    /// <inheritdoc/>
    public virtual IConfiguration? Configuration { get; init; }

    /// <inheritdoc/>
    public virtual IEnumerable<IMiddleware> Middlewares { get; init; }

    /// <inheritdoc/>
    public virtual AdapterOptions? Options { get; init; }
}
