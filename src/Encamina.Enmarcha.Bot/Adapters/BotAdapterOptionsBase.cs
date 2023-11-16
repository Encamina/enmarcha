using Encamina.Enmarcha.Bot.Abstractions.Adapters;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

namespace Encamina.Enmarcha.Bot.Adapters;

/// <summary>
/// Common options for a bot adapter. <b>This class is abstract.</b>
/// </summary>
public class BotAdapterOptionsBase : IBotAdapterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotAdapterOptionsBase"/> class.
    /// </summary>
    /// <param name="botFrameworkAuthentication">An optional environment (usually a cloud environment) used to authenticate Bot Framework Protocol network calls.</param>
    /// <param name="botTelemetryClient">An optional bot telemetry client.</param>
    /// <param name="botStates">An optional collection of bot states.</param>
    /// <param name="botMiddlewares">An optional collection of bot middlewares.</param>
    protected BotAdapterOptionsBase(BotFrameworkAuthentication botFrameworkAuthentication, IBotTelemetryClient botTelemetryClient, IEnumerable<BotState> botStates, IEnumerable<IMiddleware> botMiddlewares)
    {
        BotFrameworkAuthentication = botFrameworkAuthentication;
        BotTelemetryClient = botTelemetryClient;
        BotStates = botStates;
        Middlewares = botMiddlewares;
    }

    /// <inheritdoc/>
    public virtual BotFrameworkAuthentication BotFrameworkAuthentication { get; init; }

    /// <inheritdoc/>
    public virtual IBotTelemetryClient BotTelemetryClient { get; init; }

    /// <inheritdoc/>
    public virtual IEnumerable<BotState> BotStates { get; init; }

    /// <inheritdoc/>
    public virtual IEnumerable<IMiddleware> Middlewares { get; init; }
}
