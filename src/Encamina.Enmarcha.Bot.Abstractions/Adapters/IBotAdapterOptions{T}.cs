using Microsoft.Bot.Builder;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Abstractions.Adapters;

/// <summary>
/// Represents common options for a specific bot adapter type.
/// </summary>
/// <typeparam name="T">The type of the specific bot adapter.</typeparam>
public interface IBotAdapterOptions<out T> : IBotAdapterOptions
    where T : BotAdapter
{
    /// <summary>
    /// Gets a logger for the bot adapter type.
    /// </summary>
    ILogger<T> Logger { get; }
}
