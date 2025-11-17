using Encamina.Enmarcha.Agents.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Middleware that adds structured logging scope for AI-related requests.
/// It extracts specific headers from the request (e.g., ActivityId, UserId)
/// and includes them in the log scope for better traceability.
/// </summary>
internal sealed class LogAIRequestScopeMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<LogAIRequestScopeMiddleware> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogAIRequestScopeMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the HTTP request pipeline.</param>
    /// <param name="logger">The logger instance used to log information.</param>
    public LogAIRequestScopeMiddleware(RequestDelegate next, ILogger<LogAIRequestScopeMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to log the request scope for AI-related endpoints.
    /// If the request targets an AI endpoint, a logging scope is created with relevant request data.
    /// </summary>
    /// <param name="context">The HTTP context of the request.</param>
    /// <returns>A task representing the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata
                .GetMetadata<Microsoft.AspNetCore.Routing.EndpointGroupNameAttribute>()?
                .EndpointGroupName == CommonConstants.LogAIRequestScopeItems.AI)
        {
            var scopeState = new Dictionary<string, object> {
            { CommonConstants.LogAIRequestScopeItems.ActivityId, context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderActivityId) },
            { CommonConstants.LogAIRequestScopeItems.ConversationId, context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderConversationId) },
            { CommonConstants.LogAIRequestScopeItems.UserId, context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderUserId) },
            { CommonConstants.LogAIRequestScopeItems.UserEmail, context.GetRequestHeaderValueOrDefault(CommonConstants.LogAIRequestScopeItems.HeaderUserEmail) },
        };

            using var _ = logger.BeginScope(scopeState);
            await next(context);
        }
        else
        {
            await next(context);
        }
    }
}
