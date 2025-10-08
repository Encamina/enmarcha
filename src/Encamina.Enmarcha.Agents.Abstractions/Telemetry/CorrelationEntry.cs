namespace Encamina.Enmarcha.Agents.Abstractions.Telemetry;

/// <summary>
/// Represents a correlation entry containing trace information.
/// </summary>
/// <param name="TraceParent">The W3C traceparent value.</param>
/// <param name="TraceState">The W3C tracestate value (optional).</param>
public sealed record CorrelationEntry(string TraceParent, string? TraceState);