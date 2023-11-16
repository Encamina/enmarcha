using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.Net.Http.Extensions;

/// <summary>
/// Extension helper methods when working with an <see cref="HttpContext"/>.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Reads values from the request header.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> with the request to read header values from.</param>
    /// <param name="headerName">The name of the header to read.</param>
    /// <param name="defaultValue">A default value to return if the header is not found.</param>
    /// <returns>The value read from the header of the request.</returns>
    public static string ReadValueFromRequestHeader(this HttpContext httpContext, string headerName, string defaultValue)
    {
        Guard.IsNotNull(httpContext);
        Guard.IsNotNull(headerName);

        return httpContext.Request.Headers.TryGetValue(headerName, out var headerValue) && headerValue.Any()
            ? headerValue[0].TrimAndAsNullIfEmpty() ?? defaultValue
            : defaultValue;
    }
}
