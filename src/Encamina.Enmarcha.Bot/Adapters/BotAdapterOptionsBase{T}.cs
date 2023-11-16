using Encamina.Enmarcha.Bot.Abstractions.Adapters;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Adapters;

/// <summary>
/// Common options for specific given bot adapter type. <b>This class is abstract.</b>
/// </summary>
/// <typeparam name="T">The type of the specific bot adapter.</typeparam>
public class BotAdapterOptionsBase<T> : BotAdapterOptionsBase, IBotAdapterOptions<T>
    where T : BotAdapter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BotAdapterOptionsBase{T}"/> class.
    /// </summary>
    /// <param name="botFrameworkAuthentication">An optional environment (usually, a cloud environment) used to authenticate Bot Framework Protocol network calls.</param>
    /// <param name="botTelemetryClient">An optional bot telemetry client.</param>
    /// <param name="botStates">An optional collection of bot states.</param>
    /// <param name="botMiddlewares">An optional collection of bot middlewares.</param>
    /// <param name="logger">An optional logger fot the adapter.</param>
    protected BotAdapterOptionsBase(BotFrameworkAuthentication botFrameworkAuthentication, IBotTelemetryClient botTelemetryClient, IEnumerable<BotState> botStates, IEnumerable<IMiddleware> botMiddlewares, ILogger<T> logger)
        : base(botFrameworkAuthentication, botTelemetryClient, botStates, botMiddlewares)
    {
        Logger = logger;
    }

    /// <inheritdoc/>
    public ILogger<T> Logger { get; init; }
}
