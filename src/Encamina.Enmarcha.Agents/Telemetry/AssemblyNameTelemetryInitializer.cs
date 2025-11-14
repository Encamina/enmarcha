using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// Telemetry initializer that adds the assembly name to all custom events.
/// </summary>
public class AssemblyNameTelemetryInitializer : ITelemetryInitializer
{
    private readonly string assemblyName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyNameTelemetryInitializer"/> class.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to include in telemetry.</param>
    public AssemblyNameTelemetryInitializer(string assemblyName)
    {
        this.assemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
    }

    /// <summary>
    /// Initializes the telemetry item by adding the assembly name to custom events.
    /// </summary>
    /// <param name="telemetry">The telemetry item to initialize.</param>
    public void Initialize(ITelemetry telemetry)
    {
        if (telemetry is EventTelemetry eventTelemetry)
        {
            eventTelemetry.Properties["AssemblyName"] = assemblyName;
        }
    }
}
