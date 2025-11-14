using Microsoft.AspNetCore.Http;

namespace Encamina.Enmarcha.Agents.Extensions;

/// <summary>
/// Extension methods for HttpContext related operations.
/// </summary>
public static class HttpContextExtensions
{
    private const string NotAvailable = @"N/A";

    /// <summary>
    /// Gets the first value of the specified header from the request's headers. Returns the specified default value if the header is not present.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> instance.</param>
    /// <param name="headerName">The name of the header to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the header is not present.</param>
    /// <returns>The value of the specified header or the default value if the header is not present.</returns>
    public static string GetRequestHeaderValueOrDefault(this HttpContext? context, string headerName, string defaultValue = NotAvailable)
    {
        return context != null && context.Request.Headers.TryGetValue(headerName, out var values) ? values.FirstOrDefault() ?? defaultValue : defaultValue;
    }

    /// <summary>
    /// Tries to get the value of the specified header from the request's headers.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> instance.</param>
    /// <param name="headerName">The name of the header to retrieve.</param>
    /// <param name="value">The value of the specified header, if present; otherwise, an empty string.</param>
    /// <returns>True if the header is present, false otherwise.</returns>
    public static bool TryGetRequestHeaderValue(this HttpContext? context, string headerName, out string value)
    {
        if (context != null && context.Request.Headers.TryGetValue(headerName, out var values))
        {
            var firstNonNullValue = values.FirstOrDefault(v => v != null);

            if (firstNonNullValue != null)
            {
                value = firstNonNullValue;
                return true;
            }
        }

        value = string.Empty;
        return false;
    }
}
