using Encamina.Enmarcha.AspNet.Mvc.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure ASP.NET MVC and realted services.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds basic API Key autorization using configuration parameters from the current set of key-value application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBasicApiKeyAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<BasicApiKeyOptions>().Bind(configuration.GetSection(nameof(BasicApiKeyOptions))).ValidateDataAnnotations().ValidateOnStart();

        return services.AddBasicApiKeyAuthorization();
    }

    /// <summary>
    /// Adds basic API Key authorization using an action to configure option parameters as a singleton of <see cref="IApiKeyProvider"/> service.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">An action to configure options for the basic API Key authorization.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBasicApiKeyAuthorization(this IServiceCollection services, Action<BasicApiKeyOptions> options)
    {
        services.AddOptions<BasicApiKeyOptions>().Configure(options).ValidateDataAnnotations().ValidateOnStart();

        return services.AddBasicApiKeyAuthorization();
    }

    /// <summary>
    /// Adds a dummy API Key authorization as a sigleton of <see cref="IApiKeyProvider"/> service.
    /// </summary>
    /// <remarks>
    /// This dummy API Key provider always returns <see langword="true"/> as long as the expected API Key parameters (usually a client unique
    /// identifier and API key) are not <see langword="null"/>, empty, or whitespace.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddDummyApiKeyProvider(this IServiceCollection services)
    {
        return services.AddSingleton<IApiKeyProvider, DummyApiKeyProvider>();
    }

    private static IServiceCollection AddBasicApiKeyAuthorization(this IServiceCollection services)
    {
        return services.AddSingleton<IApiKeyProvider, BasicApiKeyProvider>();
    }
}
