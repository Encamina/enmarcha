using Encamina.Enmarcha.Agents.Abstractions.Middlewares;

using Microsoft.Agents.Builder;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Defines a usage rule for an agent middleware.
/// </summary>
/// <typeparam name="TMiddleware">
/// The type of agent middleware. It must implement the <see cref="IMiddleware"/> interface.
/// </typeparam>
public class MiddlewareUseRule<TMiddleware> : IMiddlewareUseRule
    where TMiddleware : class, IMiddleware
{
    /// <inheritdoc/>
    public bool IncludeCondition { get; init; } = true;

    /// <summary>
    /// Gets the inclusion or usage order for this middleware rule.
    /// </summary>
    public int Order { get; init; }

    /// <inheritdoc/>
    public Type MiddlewareType => typeof(TMiddleware);
}
