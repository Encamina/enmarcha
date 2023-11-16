using Encamina.Enmarcha.Bot.Abstractions.Middlewares;

using Microsoft.Bot.Builder;

namespace Encamina.Enmarcha.Bot.Middlewares;

/// <summary>
/// Defines a usage rule for a bot middleware.
/// </summary>
/// <typeparam name="TMiddleware">
/// The type of bot middleware. It must implement the <see cref="IMiddleware"/> interface.
/// </typeparam>
public class MiddlewareUseRule<TMiddleware> : IMiddlewareUseRule
    where TMiddleware : class, IMiddleware
{
    /// <inheritdoc/>
    public bool IncludeCondition { get; init; } = true;

    /// <summary>
    /// Gets the inclusion or useage order for this middleware rule.
    /// </summary>
    public int Order { get; init; }

    /// <inheritdoc/>
    public Type MiddlewareType => typeof(TMiddleware);
}
