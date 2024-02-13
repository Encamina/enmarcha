using System.Net;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AspNet.OpenApi.Middlewares;

/// <summary>
/// A middleware that authorizes requests for specific OpenAPI specifications based on a key associated with a group name of an API.
/// </summary>
public class GroupNameKeyAuthorizationMiddleware
{
    private readonly RequestDelegate next;
    private readonly GroupNameKeyAuthenticationOptions options;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GroupNameKeyAuthorizationMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <param name="options">Option parameters for the group name key authorization middleware.</param>
    /// <param name="loggerFactory">A factory for <see cref="ILogger"/> instances.</param>
    public GroupNameKeyAuthorizationMiddleware(RequestDelegate next, IOptions<GroupNameKeyAuthenticationOptions> options, ILoggerFactory loggerFactory)
    {
        this.next = next;
        this.options = options.Value;

        logger = loggerFactory.CreateLogger<GroupNameKeyAuthorizationMiddleware>();
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the execution of this middleware.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!options.IsEnabled)
        {
            logger.LogInformation(@"Swagger group name key authorization middleware is disabled!");
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }

        var requestPathValue = context.Request.Path.Value;

        if (requestPathValue == null || !requestPathValue.EndsWith(options.SpecificationFileName, StringComparison.OrdinalIgnoreCase))
        {
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }

        var groupNameKey = options.GroupNameKeys.FirstOrDefault(item => requestPathValue.StartsWith($@"{options.SpecificationPath}/{item.Key}", StringComparison.OrdinalIgnoreCase)).Value;

        if (groupNameKey == null || (context.Request.Headers.TryGetValue(Constants.OpenApi.KeyHeader, out var apiKey) && apiKey.Equals(groupNameKey)))
        {
            await next.Invoke(context).ConfigureAwait(false);
            return;
        }

        logger.LogInformation(@"Invalid or missing key for path {RequestPath}!", requestPathValue);

        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    }
}