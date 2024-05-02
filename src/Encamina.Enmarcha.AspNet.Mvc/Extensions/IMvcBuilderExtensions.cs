using Encamina.Enmarcha.AspNet.Mvc.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up MVC services in an <see cref="IMvcBuilder"/>.
/// </summary>
public static class IMvcBuilderExtensions
{
    /// <summary>
    /// Adds an API Key Authorization filter into the <see cref="MvcOptions"/> filters collection
    /// using configuration parameters from the current set of key-value application configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IMvcBuilder"/> so that additional calls can be chained.</returns>
    /// <seealso cref="IAsyncAuthorizationFilter"/>
    public static IMvcBuilder AddApiKeyAuthorizationFilter(this IMvcBuilder builder, IConfiguration configuration)
    {
        return builder.AddApiKeyAuthorizationFilter(s => s.Bind(configuration.GetSection(nameof(ApiKeyAuthorizationFilterOptions))).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds an API Key Authorization filter into the <see cref="MvcOptions"/> filters collection using an action to configure option parameters.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <param name="options">An action to configure options for the <see cref="ApiKeyAsyncAuthorizationFilter"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/> so that additional calls can be chained.</returns>
    /// <seealso cref="IAsyncAuthorizationFilter"/>
    public static IMvcBuilder AddApiKeyAuthorizationFilter(this IMvcBuilder builder, Action<ApiKeyAuthorizationFilterOptions> options)
    {
        return builder.AddApiKeyAuthorizationFilter(s => s.Configure(options).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds an API Key Authorization filter into the <see cref="MvcOptions"/> filters collection.
    /// </summary>
    /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
    /// <returns>The <see cref="IMvcBuilder"/> so that additional calls can be chained.</returns>
    /// <seealso cref="IAsyncAuthorizationFilter"/>
    public static IMvcBuilder AddApiKeyAuthorizationFilter(this IMvcBuilder builder)
    {
        return builder.AddApiKeyAuthorizationFilter(setupOptions: null);
    }

    private static IMvcBuilder AddApiKeyAuthorizationFilter(this IMvcBuilder builder, Action<Options.OptionsBuilder<ApiKeyAuthorizationFilterOptions>>? setupOptions)
    {
        var options = builder.Services.AddOptions<ApiKeyAuthorizationFilterOptions>();

        setupOptions?.Invoke(options);

        return builder.AddMvcOptions(options =>
        {
            options.Filters.Add<ApiKeyAsyncAuthorizationFilter>(0);
        });
    }
}
