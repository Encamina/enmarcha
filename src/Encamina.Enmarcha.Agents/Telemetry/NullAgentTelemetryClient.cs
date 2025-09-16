using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// A null agent telemetry client that implements <see cref="IAgentTelemetryClient"/>.
/// </summary>
public class NullAgentTelemetryClient : IAgentTelemetryClient
{
    /// <summary>
    /// Gets a new instance of NullAgentTelemetryClient.
    /// </summary>
    /// <value>
    /// A new instance of NullAgentTelemetryClient.
    /// </value>
    public static IAgentTelemetryClient Instance { get; } = new NullAgentTelemetryClient();

    /// <inheritdoc/>
    public void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation, bool success, string? message = null, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
    }

    /// <inheritdoc/>
    public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data, DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
    {
    }

    /// <inheritdoc/>
    public void TrackEvent(string eventName, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
    }

    /// <inheritdoc/>
    public void TrackException(Exception exception, IDictionary<string, string>? properties = null, IDictionary<string, double>? metrics = null)
    {
    }

    /// <inheritdoc/>
    public void TrackTrace(string message, Severity severityLevel, IDictionary<string, string> properties)
    {
    }

    /// <inheritdoc/>
    public void Flush()
    {
    }
}