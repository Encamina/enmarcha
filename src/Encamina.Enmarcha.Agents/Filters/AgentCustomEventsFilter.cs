using System.Diagnostics;

using Encamina.Enmarcha.Agents.Models;

using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Encamina.Enmarcha.Agents.Filters;

/// <summary>
/// Action filter that generates custom events for AIController requests with AgentRequest parameters.
/// </summary>
public class AgentCustomEventsFilter : IActionFilter
{
    private const string TimestampKey = "CustomEventsFilter_Timestamp";
    private const string ActionStartEvent = "AgentStart";
    private const string ActionEndEvent = "AgentEnd";
    private const string ActionErrorEvent = "AgentError";

    private readonly TelemetryClient telemetryClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AgentCustomEventsFilter"/> class.
    /// </summary>
    /// <param name="telemetryClient">The Application Insights telemetry client.</param>
    public AgentCustomEventsFilter(TelemetryClient telemetryClient)
    {
        this.telemetryClient = telemetryClient;
    }

    /// <summary>
    /// Called before the action executes.
    /// </summary>
    /// <param name="context">The action executing context.</param>
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!ShouldTrack(context))
        {
            return;
        }

        context.HttpContext.Items[TimestampKey] = Stopwatch.GetTimestamp();

        var properties = new Dictionary<string, string>();

        var agentRequest = context.ActionArguments.Values.OfType<AgentRequest>().FirstOrDefault();
        if (agentRequest is not null)
        {
            properties["Input"] = agentRequest.Input;
            properties["Locale"] = agentRequest.Locale;
        }

        telemetryClient.TrackEvent(ActionStartEvent, properties);
    }

    /// <summary>
    /// Called after the action executes.
    /// </summary>
    /// <param name="context">The action executed context.</param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (!ShouldTrack(context) ||
            !context.HttpContext.Items.TryGetValue(TimestampKey, out var startTimestampObj) ||
            startTimestampObj is not long startTimestamp)
        {
            return;
        }

        var durationMs = Stopwatch.GetElapsedTime(startTimestamp, Stopwatch.GetTimestamp()).TotalMilliseconds;
        var properties = new Dictionary<string, string>
        {
            { "DurationMs", durationMs.ToString("F2") },
        };

        var statusCode = context.HttpContext.Response.StatusCode;
        properties["StatusCode"] = statusCode.ToString();

        // Track error event if status code is not 200
        if (statusCode != StatusCodes.Status200OK)
        {
            var errorProperties = new Dictionary<string, string>(properties);

            if (context.Exception is not null)
            {
                errorProperties["ExceptionType"] = context.Exception.GetType().Name;
                errorProperties["ExceptionMessage"] = context.Exception.Message;
            }

            if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult { Value: not null } errorResult)
            {
                errorProperties["ErrorResponse"] = errorResult.Value.ToString() ?? string.Empty;
            }

            telemetryClient.TrackEvent(ActionErrorEvent, errorProperties);
        }

        if (context.Result is Microsoft.AspNetCore.Mvc.ObjectResult { Value: not null } objectResult)
        {
            var textProperty = objectResult.Value.GetType().GetProperty("Text");
            if (textProperty?.GetValue(objectResult.Value) is string textValue && !string.IsNullOrEmpty(textValue))
            {
                properties["Output"] = textValue;
            }
        }

        telemetryClient.TrackEvent(ActionEndEvent, properties);
        context.HttpContext.Items.Remove(TimestampKey);
    }

    /// <summary>
    /// Determines if the request should be tracked based on controller name and parameter types.
    /// </summary>
    /// <param name="context">The filter context.</param>
    /// <returns>True if the request should be tracked; otherwise, false.</returns>
    private static bool ShouldTrack(FilterContext context) =>
        context.ActionDescriptor.RouteValues.TryGetValue("controller", out var controller) &&
        controller?.Equals("AI", StringComparison.OrdinalIgnoreCase) == true &&
        context.ActionDescriptor.Parameters.Any(p => typeof(AgentRequest).IsAssignableFrom(p.ParameterType));
}
