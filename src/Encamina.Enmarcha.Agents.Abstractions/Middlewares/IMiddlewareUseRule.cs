using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Agents.Abstractions.Middlewares;

/// <summary>
/// Represents a usage rule for an agent middleware.
/// </summary>
public interface IMiddlewareUseRule : IOrderable
{
    /// <summary>
    /// Gets a value indicating whether to include or not the middleware.
    /// </summary>
    bool IncludeCondition { get; init; }

    /// <summary>
    /// Gets the type of the middleware.
    /// </summary>
    Type MiddlewareType { get; }
}
