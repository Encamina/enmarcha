using System.Diagnostics.Tracing;
using System.Reflection;

namespace Encamina.Enmarcha.Bot.Logging;

/// <summary>
/// Event source for logging to Application Insights.
/// </summary>
[EventSource(Name = "Microsoft-ApplicationInsights-ConversationScoped-LoggerProvider")]
internal sealed class ApplicationInsightsConversationScopedLoggerEventSource : EventSource
{
    /// <summary>
    /// The instance of the logger event source.
    /// </summary>
    public static readonly ApplicationInsightsConversationScopedLoggerEventSource Log = new();

    private readonly string applicationName;

    private ApplicationInsightsConversationScopedLoggerEventSource()
    {
        applicationName = GetApplicationName();
    }

    /// <summary>
    /// Logs an error message to Application Insights.
    /// </summary>
    /// <param name="error">The error message to log.</param>
    /// <param name="applicationName">The name of the application.</param>
    [Event(1, Message = "Sending log to ApplicationInsigthsConversationScopedLoggerProvider has failed. Error: {0}", Level = EventLevel.Error)]
    public void FailedToLog(string error, string? applicationName = null)
    {
        WriteEvent(1, error, applicationName ?? this.applicationName);
    }

    /// <summary>
    /// Gets the name of the entry assembly.
    /// </summary>
    /// <returns>The name of the entry assembly.</returns>
    [NonEvent]
    private static string GetApplicationName()
    {
        try
        {
            return Assembly.GetEntryAssembly()!.GetName().Name;
        }
        catch
        {
            return "Unknown";
        }
    }
}
