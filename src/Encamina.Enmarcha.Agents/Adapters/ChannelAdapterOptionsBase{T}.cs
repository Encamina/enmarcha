using Encamina.Enmarcha.Agents.Abstractions.Adapters;
using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Hosting.AspNetCore.BackgroundQueue;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Adapters;

/// <summary>
/// Common options for specific given agent adapter type.
/// </summary>
/// <typeparam name="T">The type of the specific agent adapter.</typeparam>
public class ChannelAdapterOptionsBase<T> : ChannelAdapterOptionsBase, IChannelAdapterOptions<T>
    where T : IChannelAdapter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelAdapterOptionsBase{T}"/> class.
    /// </summary>
    /// <param name="activityTaskQueue">An activity task queue to manage the delivery of activities.</param>
    /// <param name="channelServiceClientFactory">An environment (usually a cloud environment) used to authenticate Activity Protocol network calls.</param>
    /// <param name="agentTelemetryClient">An agent telemetry client.</param>
    /// <param name="agentStates">A collection of agent states.</param>
    /// <param name="agentMiddlewares">A collection of agent middlewares.</param>
    /// <param name="logger">An optional logger for the adapter.</param>
    protected ChannelAdapterOptionsBase(IActivityTaskQueue activityTaskQueue, IChannelServiceClientFactory channelServiceClientFactory,
        IAgentTelemetryClient agentTelemetryClient, IEnumerable<AgentState> agentStates, IMiddleware[] agentMiddlewares, ILogger<T> logger)
        : base(activityTaskQueue, channelServiceClientFactory, agentTelemetryClient, agentStates, agentMiddlewares)
    {
        Logger = logger;
    }

    /// <inheritdoc/>
    public ILogger<T> Logger { get; init; }
}
