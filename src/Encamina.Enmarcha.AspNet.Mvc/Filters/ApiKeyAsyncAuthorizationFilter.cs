using System.Reflection;
using System.Security.Claims;

using Encamina.Enmarcha.AspNet.Mvc.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AspNet.Mvc.Filters;

/// <summary>
/// Asynchronous API Key authorization filter.
/// </summary>
internal sealed class ApiKeyAsyncAuthorizationFilter : IAsyncAuthorizationFilter, IOrderedFilter
{
    private const string AuthenticationType = @"ApiKey";

    private readonly ApiKeyAuthorizationFilterOptions options;
    private readonly IApiKeyProvider apiKeyService;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAsyncAuthorizationFilter"/> class.
    /// </summary>
    /// <param name="apiKeyService">A valid instance of a <see cref="IApiKeyProvider"/> used for authorization with this filter.</param>
    /// <param name="options">Configuration options for this filter.</param>
    /// <param name="logger">A logger for this filter.</param>
    public ApiKeyAsyncAuthorizationFilter(IApiKeyProvider apiKeyService, IOptions<ApiKeyAuthorizationFilterOptions> options, ILogger<ApiKeyAsyncAuthorizationFilter> logger)
    {
        this.apiKeyService = apiKeyService;
        this.logger = logger;
        this.options = options.Value;
    }

    /// <inheritdoc/>
    public int Order => 0;

    /// <inheritdoc/>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var logName = nameof(ApiKeyAsyncAuthorizationFilter);

        var apiKeyName = GetApiKeyName(context.ActionDescriptor);

        if (string.IsNullOrWhiteSpace(apiKeyName))
        {
            return;
        }

        if (context.HttpContext.Request.Headers.TryGetValue(options.HeaderApiKey, out var apiKey) && !string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogTrace($@"{logName} | Found '{options.HeaderApiKey}' header. Starting API Key validation...");

            if (await apiKeyService.IsAuthorizedAsync(apiKeyName, apiKey, context.HttpContext.RequestAborted))
            {
                logger.LogTrace($@"{logName} | Client {apiKeyName} successfully logged in with key {apiKey}");

                context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(AuthenticationType.ToLowerInvariant(), apiKey),
                    new Claim(ClaimTypes.Name, apiKeyName),
                }, AuthenticationType));

                return;
            }

            logger.LogError($@"{logName} | ERROR | Invalid API key for API key name '{apiKeyName}'!");
        }

        logger.LogError($@"{logName} | ERROR | Missing {options.HeaderApiKey} header!");

        context.Result = new UnauthorizedResult();
    }

    private static string GetApiKeyName(ActionDescriptor descriptor)
    {
        return descriptor is ControllerActionDescriptor controllerActionDescriptor
            ? GetApiKeyName(controllerActionDescriptor.MethodInfo) ?? GetApiKeyName(controllerActionDescriptor.ControllerTypeInfo)
            : null;
    }

    private static string GetApiKeyName(MemberInfo memberInfo)
    {
        return memberInfo?.GetCustomAttribute<ApiKeyAttribute>(true)?.ApiKeyName;
    }
}
