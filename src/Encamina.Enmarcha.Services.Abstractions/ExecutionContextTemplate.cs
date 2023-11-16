using System.Globalization;

using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// A template that can be used to create or configure an <see cref="IExecutionContext"/>.
/// </summary>
public class ExecutionContextTemplate : IExecutionContextTemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContextTemplate"/> class.
    /// </summary>
    public ExecutionContextTemplate()
    {
    }

    /// <inheritdoc/>
    public virtual string CorrelationId { get; init; }

    /// <inheritdoc/>
    public virtual string CorrelationCallId { get; init; }

    /// <inheritdoc/>
    public virtual CultureInfo CultureInfo { get; init; }

    /// <inheritdoc/>
    public virtual CancellationToken CancellationToken { get; init; }

    /// <inheritdoc/>
    public virtual IConfiguration Configuration { get; init; }
}
