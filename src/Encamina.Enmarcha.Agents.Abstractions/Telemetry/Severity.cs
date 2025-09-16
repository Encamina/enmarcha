namespace Encamina.Enmarcha.Agents.Abstractions.Telemetry;

/// <summary>
/// Defines trace severity levels for use with a <see cref="IAgentTelemetryClient"/> object.
/// </summary>
public enum Severity
{
    /// <summary>
    /// Verbose severity level.
    /// </summary>
    Verbose = 0,

    /// <summary>
    /// Information severity level.
    /// </summary>
    Information = 1,

    /// <summary>
    /// Warning severity level.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Error severity level.
    /// </summary>
    Error = 3,

    /// <summary>
    /// Critical severity level.
    /// </summary>
    Critical = 4,
}