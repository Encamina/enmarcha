using Encamina.Enmarcha.AspNet.OpenApi.Middlewares;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AspNet.OpenApi.Extensions;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Use the OpenAPI group name key authorization middleware with a given configuration.
    /// </summary>
    /// <param name="app">A valid instance of <see cref="IApplicationBuilder"/> as the application's request pipeline builder.</param>
    /// <param name="setupAction">An optional setup action for the <see cref="GroupNameKeyAuthenticationOptions"/>.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseOpenApiGroupNameKeyAuthorization(this IApplicationBuilder app, Action<GroupNameKeyAuthenticationOptions> setupAction)
    {
        GroupNameKeyAuthenticationOptions options;

        using (var scope = app.ApplicationServices.CreateScope())
        {
            options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<GroupNameKeyAuthenticationOptions>>().Value;

            setupAction?.Invoke(options);
        }

        return app.UseOpenApiGroupNameKeyAuthorization(options);
    }

    /// <summary>
    /// Use the OpenAPI group name key authorization middleware with options.
    /// </summary>
    /// <param name="app">A valid instance of <see cref="IApplicationBuilder"/> as the application's request pipeline builder.</param>
    /// <param name="options">Option parameters for the group name key authorization middleware.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseOpenApiGroupNameKeyAuthorization(this IApplicationBuilder app, GroupNameKeyAuthenticationOptions options)
    {
        return app.UseMiddleware<GroupNameKeyAuthorizationMiddleware>(Options.Create(options));
    }

    /// <summary>
    /// Use the OpenAPI group name key authorization middleware.
    /// </summary>
    /// <param name="app">A valid instance of <see cref="IApplicationBuilder"/> as the application's request pipeline builder.</param>
    /// <returns>The <see cref="IApplicationBuilder"/> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseOpenApiGroupNameKeyAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GroupNameKeyAuthorizationMiddleware>();
    }
}
