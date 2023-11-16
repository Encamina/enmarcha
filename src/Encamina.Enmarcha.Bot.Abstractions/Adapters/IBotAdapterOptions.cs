using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

namespace Encamina.Enmarcha.Bot.Abstractions.Adapters;

/// <summary>
/// Represents common options for a bot adapter.
/// </summary>
public interface IBotAdapterOptions
{
    /// <summary>
    /// Gets the environment (usually a cloud environment) used to authenticate Bot Framework Protocol network calls.
    /// </summary>
    BotFrameworkAuthentication BotFrameworkAuthentication { get; init; }

    /// <summary>
    /// Gets a bot telemetry client.
    /// </summary>
    IBotTelemetryClient BotTelemetryClient { get; init; }

    /// <summary>
    /// Gets the collection of bot states.
    /// </summary>
    IEnumerable<BotState> BotStates { get; init; }

    /// <summary>
    /// Gets the collection of current bot middlewares.
    /// </summary>
    IEnumerable<IMiddleware> Middlewares { get; init; }
}
