using System.Globalization;

using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Services.Abstractions;

/// <summary>
/// Represents the template that can be used to create or configure an <see cref="IExecutionContext"/>.
/// </summary>
public interface IExecutionContextTemplate
{
    /// <summary>
    /// Gets the correlation unique identifier to identify a call chain from this execution context.
    /// </summary>
    string CorrelationId { get; init; }

    /// <summary>
    /// Gets the correlation call unique identifier to identify a single call from other calls in a chain from this execution context.
    /// </summary>
    string CorrelationCallId { get; init; }

    /// <summary>
    /// Gets the current culture information used in this context.
    /// </summary>
    CultureInfo CultureInfo { get; init; }

    /// <summary>
    /// Gets the token to monitor notifications of an operation cancellation.
    /// </summary>
    CancellationToken CancellationToken { get; init; }

    /// <summary>
    /// Gets the current set of key-value application configuration parameters.
    /// </summary>
    public IConfiguration Configuration { get; init; }
}
