using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Hosting.AspNetCore;
using Microsoft.Agents.Hosting.AspNetCore.BackgroundQueue;
using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Agents.Abstractions.Adapters;

/// <summary>
/// Represents common options for a channel adapter.
/// </summary>
public interface IChannelAdapterOptions
{
    /// <summary>
    /// Gets the activity task queue.
    /// </summary>
    IActivityTaskQueue ActivityTaskQueue { get; init; }

    /// <summary>
    /// Gets the collection of agent states.
    /// </summary>
    IEnumerable<AgentState> AgentStates { get; init; }

    /// <summary>
    /// Gets the agent telemetry client.
    /// </summary>
    IAgentTelemetryClient AgentTelemetryClient { get; init; }

    /// <summary>
    /// Gets a factory to create channel service clients.
    /// </summary>
    IChannelServiceClientFactory ChannelServiceClientFactory { get; init; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    IConfiguration? Configuration { get; init; }

    /// <summary>
    /// Gets the collection of current agent middlewares.
    /// </summary>
    IEnumerable<IMiddleware> Middlewares { get; init; }

    /// <summary>
    /// Gets additional adapter options.
    /// </summary>
    AdapterOptions? Options { get; init; }
}
