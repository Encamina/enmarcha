using Microsoft.Agents.Builder;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Abstractions.Adapters;

/// <summary>
/// Represents common options for a specific channel adapter type.
/// </summary>
/// <typeparam name="T">The type of the specific channel adapter.</typeparam>
public interface IChannelAdapterOptions<out T> : IChannelAdapterOptions
    where T : IChannelAdapter
{
    /// <summary>
    /// Gets a logger for the channel adapter type.
    /// </summary>
    ILogger<T> Logger { get; }
}
