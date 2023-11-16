using Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle;

using Microsoft.Extensions.Options;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure OpenAPI (a.k.a. Swagger) components.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds version support for the generation of OpenAPI documents using configuration parameters from the current set of key-value application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddVersionSwaggerGenConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddVersionSwaggerGenConfigureOptions(s => s.Bind(configuration.GetSection(nameof(VersionSwaggerGenOptions))).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds version support for the generation of OpenAPI documents using using an action to configure option parameters.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">An action to configure options for the basic API Key authorization.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddVersionSwaggerGenConfigureOptions(this IServiceCollection services, Action<VersionSwaggerGenOptions> options)
    {
        return services.AddVersionSwaggerGenConfigureOptions(s => s.Configure(options).ValidateDataAnnotations().ValidateOnStart());
    }

    /// <summary>
    /// Adds version support for the generation of OpenAPI documents.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddVersionSwaggerGenConfigureOptions(this IServiceCollection services)
    {
        return services.AddVersionSwaggerGenConfigureOptions(setupOptions: null);
    }

    private static IServiceCollection AddVersionSwaggerGenConfigureOptions(this IServiceCollection services, Action<OptionsBuilder<VersionSwaggerGenOptions>> setupOptions)
    {
        var options = services.AddOptions<VersionSwaggerGenOptions>();

        setupOptions?.Invoke(options);

        return services.AddTransient<IConfigureOptions<SwaggerGenOptions>, VersionSwaggerGenConfigureOptions>();
    }
}
