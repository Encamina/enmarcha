using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Email.MailKit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable S2360 // Optional parameters should not be used

/// <summary>
/// Extension methods for configuring common and required services e-mail management using <see href="https://github.com/jstedfast/MailKit">MailKit.</see>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for <see href="https://github.com/jstedfast/MailKit">MailKit</see> as e-mail provider,
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0#remarks">as recommended by Microsoft </see>, using
    /// current configuration.
    /// </summary>
    /// <remarks>Adds the <see cref="IEmailProviderFactory"/> to retrieve instances of <see cref="IEmailProvider"/> in the given <paramref name="serviceLifetime">service lifetime</paramref>.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="serviceLifetime">The lifetime for the e-mail proider service. By default it is <see cref="ServiceLifetime.Singleton"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <seealso href="http://www.mimekit.net/"/>
    public static IServiceCollection AddMailKitEmailProvider(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.InnerAddMailKitEmailProvider(optionBuilder => optionBuilder.Bind(configuration?.GetSection(nameof(SmtpClientOptions))).ValidateDataAnnotations().ValidateOnStart(), serviceLifetime);
    }

    /// <summary>
    /// Adds support for <see href="https://github.com/jstedfast/MailKit">MailKit</see> as e-mail provider,
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0#remarks">as recommended by Microsoft </see>, using
    /// given options.
    /// </summary>
    /// <remarks>Adds the <see cref="IEmailProviderFactory"/> to retrieve instances of <see cref="IEmailProvider"/> in the given <paramref name="serviceLifetime">service lifetime</paramref>.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">Action to configure options for the email provider.</param>
    /// <param name="serviceLifetime">The lifetime for the e-mail proider service. By default it is <see cref="ServiceLifetime.Singleton"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <seealso href="http://www.mimekit.net/"/>
    public static IServiceCollection AddMailKitEmailProvider(this IServiceCollection services, Action<SmtpClientOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.InnerAddMailKitEmailProvider(optionBuilder => optionBuilder.Configure(options).ValidateDataAnnotations().ValidateOnStart(), serviceLifetime);
    }

    /// <summary>
    /// Adds support for <see href="https://github.com/jstedfast/MailKit">MailKit</see> as e-mail provider,
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-6.0#remarks">as recommended by Microsoft </see>, using
    /// both current configuration and given options.
    /// </summary>
    /// <remarks>Adds the <see cref="IEmailProviderFactory"/> to retrieve instances of <see cref="IEmailProvider"/> in the given <paramref name="serviceLifetime">service lifetime</paramref>.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="options">Action to configure options for the email provider.</param>
    /// <param name="serviceLifetime">The lifetime for the e-mail proider service. By default it is <see cref="ServiceLifetime.Singleton"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <seealso href="http://www.mimekit.net/"/>
    public static IServiceCollection AddMailKitEmailProvider(this IServiceCollection services, IConfiguration configuration, Action<SmtpClientOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.InnerAddMailKitEmailProvider(optionBuilder => optionBuilder.Bind(configuration?.GetSection(nameof(SmtpClientOptions))).PostConfigure(options).ValidateDataAnnotations().ValidateOnStart(), serviceLifetime);
    }

    private static IServiceCollection InnerAddMailKitEmailProvider(this IServiceCollection services, Action<OptionsBuilder<SmtpClientOptions>> setupOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        // Use an inner service collection to prevent conflicts with multiple injections of this service.
        // Also, to prevent exposing non-required or non-related services.
        var innerServiceCollection = new ServiceCollection();

        // Set-up options...
        setupOptions.Invoke(innerServiceCollection.AddOptions<SmtpClientOptions>());

        var serviceInstanceBuilder = (IServiceProvider _) =>
        {
            return new EmailService(innerServiceCollection.BuildServiceProvider().GetRequiredService<IOptions<SmtpClientOptions>>());
        };

        services.TryAddSingleton<IEmailProviderFactoryProvider, EmailServiceFactoryProvider>();

        // The service «IEmailBuilder» is not registered into the service collection to prevent consumers of this implementation to get a dependency to it.
        // Instead, to get an instance of «IEmailBuilder», consumers are expected to call the «BeginSendEmail» method from «IEmailProvider».
        return services.AddType(serviceLifetime, serviceInstanceBuilder)
                       .AddType<IEmailProvider>(serviceLifetime, serviceInstanceBuilder);
    }
}

#pragma warning restore S2360 // Optional parameters should not be used
