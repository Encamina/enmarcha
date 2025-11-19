using System.Runtime.ExceptionServices;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// A middleware that catches rate limit (429) exceptions from the Semantic Kernel and returns a 429 Too Many Requests response.
/// </summary>
public class SemanticKernelRateLimitMiddleware
{
    private const string DefaultErrorMessage = @"The request was rate limited. Please try again later.";

    private readonly RequestDelegate next;
    private readonly ILogger<SemanticKernelRateLimitMiddleware> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticKernelRateLimitMiddleware"/> class.
    /// </summary>
    /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
    /// <param name="logger">The logger for the middleware.</param>
    public SemanticKernelRateLimitMiddleware(RequestDelegate next, ILogger<SemanticKernelRateLimitMiddleware> logger)
    {
        this.logger = logger;
        this.next = next;
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the execution of this middleware.</returns>
    public Task InvokeAsync(HttpContext context)
    {
        ExceptionDispatchInfo? exceptionDispatchInfo;

        try
        {
            var task = next(context);

            return !task.IsCompletedSuccessfully
                ? Awaited(this, context, task)
                : Task.CompletedTask;
        }
        catch (HttpOperationException httpOperationException)
        {
            // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
            exceptionDispatchInfo = ExceptionDispatchInfo.Capture(httpOperationException);
        }

        return HandleExceptionAsync(context, exceptionDispatchInfo);

        static async Task Awaited(SemanticKernelRateLimitMiddleware middleware, HttpContext context, Task task)
        {
            ExceptionDispatchInfo? exceptionDispatchInfo = null;

            try
            {
                await task;
            }
            catch (HttpOperationException exception)
            {
                // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
            }

            if (exceptionDispatchInfo != null)
            {
                await middleware.HandleExceptionAsync(context, exceptionDispatchInfo);
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, ExceptionDispatchInfo exceptionDispatchInfo)
    {
        var exception = (HttpOperationException)exceptionDispatchInfo.SourceException;

        // Check if it's a 429 error based on the HttpOperationException status code
        if (exception.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            logger.LogWarning(exception, "Rate limit exceeded (429) for request to {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = @"application/json";

            string? retryAfter = null;
            if (exception.InnerException is System.ClientModel.ClientResultException azureEx)
            {
                var rawResponse = azureEx.GetRawResponse();
                if (rawResponse?.Headers.TryGetValue("Retry-After", out retryAfter) == true)
                {
                    context.Response.Headers.Append("Retry-After", retryAfter);
                }
            }

            var payload = JsonSerializer.Serialize(new
            {
                error = DefaultErrorMessage,
            });

            await context.Response.WriteAsync(payload);
            return;
        }

        // If the internal exception is not a 429, rethrow the original exception.
        exceptionDispatchInfo.Throw();
    }
}