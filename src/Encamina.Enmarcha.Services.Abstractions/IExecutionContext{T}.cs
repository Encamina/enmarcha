using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// Represents a basic execution context for a specific type.
/// </summary>
/// <typeparam name="T">The specific type of this excecution context.</typeparam>
public interface IExecutionContext<T> : IExecutionContext
    where T : class
{
    /// <summary>
    /// Gets a logger for the specific type of this execution context.
    /// </summary>
    ILogger<T> Logger { get; init; }
}
