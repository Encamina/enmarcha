using System.Globalization;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// Base class for basic execution contexts.
/// </summary>
public class ExecutionContext : IdentifiableBase<Guid>, IExecutionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
    /// </summary>
    public ExecutionContext() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext"/> class from a given template.
    /// </summary>
    /// <param name="template">An execution context template with values to configure this execution context.</param>
    public ExecutionContext(IExecutionContextTemplate? template)
    {
        Id = Guid.NewGuid();

        if (template != null)
        {
            Configuration = template.Configuration;
            CancellationToken = template.CancellationToken;
            CorrelationCallId = template.CorrelationCallId;
            CorrelationId = template.CorrelationId;
            CultureInfo = template.CultureInfo;
        }
        else
        {
            CorrelationCallId = Guid.NewGuid().ToString();
            CorrelationId = Guid.NewGuid().ToString();
            CancellationToken = CancellationToken.None;
            CultureInfo = CultureInfo.InvariantCulture;
        }
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
