using Encamina.Enmarcha.Agents.Logging;

using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Encamina.Enmarcha.Agents.Options;

/// <summary>
/// Defines the custom behavior of the tracing information sent to Application Insights using <see cref="ApplicationInsightsConversationScopedLogger"/>.
/// </summary>
public sealed class ApplicationInsightsConversationScopedLoggerOptions : ApplicationInsightsLoggerOptions
{
    /// <summary>
    /// Gets or Sets a list of event names that should be tracked. If null, all events will be tracked.
    /// </summary>
    public IEnumerable<string>? EventsToTrack { get; set; }
}
