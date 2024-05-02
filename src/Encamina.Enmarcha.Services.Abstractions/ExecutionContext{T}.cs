using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// Base class for basic execution contexts for a specific type.
/// </summary>
/// <typeparam name="T">The specific type of this excecution context.</typeparam>
public class ExecutionContext<T> : ExecutionContext, IExecutionContext<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext{T}"/> class.
    /// </summary>
    /// <param name="logger">A logger for the specific type of this excecution context.</param>
    public ExecutionContext(ILogger<T> logger) : this(null, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext{T}"/> class from a template.
    /// </summary>
    /// <param name="template">An execution context template with values to configure this execution context.</param>
    /// <param name="logger">A logger for the specific type of this excecution context.</param>
    public ExecutionContext(IExecutionContextTemplate? template, ILogger<T> logger) : base(template)
    {
        Logger = logger;
    }

    /// <inheritdoc/>
    public virtual ILogger<T> Logger { get; init; }
}
