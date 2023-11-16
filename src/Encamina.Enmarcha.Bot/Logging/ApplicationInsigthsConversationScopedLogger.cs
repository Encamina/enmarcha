using System.Globalization;
using System.Text;

using Encamina.Enmarcha.Bot.Abstractions.Extensions;
using Encamina.Enmarcha.Bot.Options;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility.Implementation.Tracing;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Logging;

/// <summary>
/// <see cref="ILogger"/> implementation that forwards log messages as Application Insight trace events.
/// Also tracks the ConversationId as a property in the telemetry.
/// </summary>
internal sealed class ApplicationInsigthsConversationScopedLogger : ILogger
{
    private const string ExceptionMessage = @"ExceptionMessage";
    private const string ExceptionStackTrace = @"ExceptionStackTrace";
    private const string FormattedMessage = @"FormattedMessage";
    private const string CategoryName = @"CategoryName";
    private const string EventId = @"EventId";
    private const string EventName = @"EventName";
    private const string ConversationId = @"ConversationId";
    private const string OriginalFormat = @"OriginalFormat";
    private const string Scope = @"Scope";

    private readonly string categoryName;
    private readonly TelemetryClient telemetryClient;
    private readonly ApplicationInsightsConversationScopedLoggerOptions applicationInsightsLoggerOptions;
    private readonly IHttpContextAccessor httpContextAccesor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationInsigthsConversationScopedLogger"/> class.
    /// </summary>
    /// <param name="categoryName">The name of the category.</param>
    /// <param name="telemetryClient">The telemetry client to use to send telemetry.</param>
    /// <param name="applicationInsightsLoggerOptions">The options object controlling logging behavior.</param>
    /// <param name="httpContextAccesor">The http context accessor to extract the ConversationId from.</param>
    public ApplicationInsigthsConversationScopedLogger(string categoryName, TelemetryClient telemetryClient, ApplicationInsightsConversationScopedLoggerOptions applicationInsightsLoggerOptions, IHttpContextAccessor httpContextAccesor)
    {
        this.categoryName = categoryName;
        this.telemetryClient = telemetryClient;
        this.applicationInsightsLoggerOptions = applicationInsightsLoggerOptions;
        this.httpContextAccesor = httpContextAccesor;
    }

    /// <summary>
    /// Gets or sets the external scope provider.
    /// </summary>
    internal IExternalScopeProvider ExternalScopeProvider { get; set; }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
        return ExternalScopeProvider != null ? ExternalScopeProvider.Push(state) : NullScope.Instance;
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None && telemetryClient.IsEnabled();
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        try
        {
            if (IsEnabled(logLevel))
            {
                if (exception == null || !applicationInsightsLoggerOptions.TrackExceptionsAsExceptionTelemetry)
                {
                    var traceTelemetry = new TraceTelemetry(formatter(state, exception), GetSeverityLevel(logLevel));
                    PopulateTelemetry(traceTelemetry, state, eventId);
                    if (exception != null)
                    {
                        traceTelemetry.Properties.Add(ExceptionMessage, exception.Message);
                        traceTelemetry.Properties.Add(ExceptionStackTrace, exception.ToInvariantString());
                    }

                    telemetryClient.TrackTrace(traceTelemetry);

                    if (applicationInsightsLoggerOptions.EventsToTrack is null || applicationInsightsLoggerOptions.EventsToTrack.Contains(eventId.Name))
                    {
                        var customEventTelemetry = new EventTelemetry(eventId.Name);
                        PopulateTelemetry(customEventTelemetry, state, eventId);
                        telemetryClient.TrackEvent(customEventTelemetry);
                    }
                }
                else
                {
                    var exceptionTelemetry = new ExceptionTelemetry(exception)
                    {
                        Message = exception.Message,
                        SeverityLevel = GetSeverityLevel(logLevel),
                    };

                    exceptionTelemetry.Properties.Add(FormattedMessage, formatter(state, exception));
                    PopulateTelemetry(exceptionTelemetry, state, eventId);
                    telemetryClient.TrackException(exceptionTelemetry);
                }
            }
        }
        catch (Exception ex)
        {
            ApplicationInsightsConversationScopedLoggerEventSource.Log.FailedToLog(ex.ToInvariantString());
        }
    }

    private static SeverityLevel GetSeverityLevel(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Critical:
                return SeverityLevel.Critical;
            case LogLevel.Error:
                return SeverityLevel.Error;
            case LogLevel.Warning:
                return SeverityLevel.Warning;
            case LogLevel.Information:
                return SeverityLevel.Information;
            case LogLevel.Debug:
            case LogLevel.Trace:
            default:
                return SeverityLevel.Verbose;
        }
    }

    private void PopulateTelemetry<TState>(ISupportProperties telemetryItem, TState state, EventId eventId)
    {
        var dict = telemetryItem.Properties;
        dict[CategoryName] = categoryName;

        if (eventId.Id != 0)
        {
            dict[EventId] = eventId.Id.ToString(CultureInfo.InvariantCulture);
        }

        if (!string.IsNullOrEmpty(eventId.Name))
        {
            dict[EventName] = eventId.Name;
        }

        var activity = httpContextAccesor.HttpContext?.Request?.GetActivityAsync()?.Result;
        if (activity is not null)
        {
            dict[ConversationId] = activity.Conversation.Id;
        }

        if (state is IReadOnlyCollection<KeyValuePair<string, object>> stateDictionary)
        {
            foreach (var item in stateDictionary)
            {
                if (item.Key == $@"{{{OriginalFormat}}}")
                {
                    dict[OriginalFormat] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                }
                else
                {
                    dict[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                }
            }
        }

        if (applicationInsightsLoggerOptions.IncludeScopes && ExternalScopeProvider is not null)
        {
            var stringBuilder = new StringBuilder();
            ExternalScopeProvider.ForEachScope(
                (activeScope, builder) =>
                {
                    // Ideally we expect that the scope to implement IReadOnlyList<KeyValuePair<string, object>>.
                    // But this is not guaranteed as user can call BeginScope and pass anything. Hence
                    // we try to resolve the scope as Dictionary and if we fail, we just serialize the object and add it.

                    if (activeScope is IReadOnlyCollection<KeyValuePair<string, object>> activeScopeDictionary)
                    {
                        foreach (var item in activeScopeDictionary)
                        {
                            if (item.Key == $@"{{{OriginalFormat}}}")
                            {
                                dict[OriginalFormat] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                dict[item.Key] = Convert.ToString(item.Value, CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    else
                    {
                        builder.Append(@" => ").Append(activeScope);
                    }
                },
                stringBuilder);

            if (stringBuilder.Length > 0)
            {
                dict[Scope] = stringBuilder.ToString();
            }
        }
    }
}
