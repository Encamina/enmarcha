using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering services into an <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Tries to adds the specified <typeparamref name="TService"/> as the given lifetime implementation type specified in <typeparamref name="TImplementation"/> into
    /// the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service (usually an abstraction like and interface type) to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection TryAddType<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.TryAddScoped<TService, TImplementation>();
                break;

            case ServiceLifetime.Singleton:
                services.TryAddSingleton<TService, TImplementation>();
                break;

            case ServiceLifetime.Transient:
                services.TryAddTransient<TService, TImplementation>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Tries to add the specified <typeparamref name="TService"/> as the given lifetime using the factory specified in <paramref name="implementationFactory"/> for the type
    /// specified in <typeparamref name="TImplementation"/> into the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service (usually an abstraction like and interface type) to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection TryAddType<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime, Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.TryAddScoped<TService>(implementationFactory);
                break;

            case ServiceLifetime.Singleton:
                services.TryAddSingleton<TService>(implementationFactory);
                break;

            case ServiceLifetime.Transient:
                services.TryAddTransient<TService>(implementationFactory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Tries to add the specified <typeparamref name="T"/> as the given lifetime with an instance specified in <paramref name="implementationInstance"/> to the collection
    /// of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationInstance">The instance of the service to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection TryAddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime, T implementationInstance)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.TryAddScoped(sp => implementationInstance);
                break;

            case ServiceLifetime.Singleton:
                services.TryAddSingleton(implementationInstance);
                break;

            case ServiceLifetime.Transient:
                services.TryAddTransient(sp => implementationInstance);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Tries to add the specified <typeparamref name="T"/> as the given lifetime using the given factory specified in <paramref name="implementationFactory"/> into the
    /// collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection TryAddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime, Func<IServiceProvider, T> implementationFactory)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.TryAddScoped(implementationFactory);
                break;

            case ServiceLifetime.Singleton:
                services.TryAddSingleton(implementationFactory);
                break;

            case ServiceLifetime.Transient:
                services.TryAddTransient(implementationFactory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Tries to add the specified <typeparamref name="T"/> as the given lifetime to the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection TryAddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.TryAddScoped<T>();
                break;

            case ServiceLifetime.Singleton:
                services.TryAddSingleton<T>();
                break;

            case ServiceLifetime.Transient:
                services.TryAddTransient<T>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="TService"/> as the given lifetime implementation type specified in <typeparamref name="TImplementation"/> into
    /// the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service (usually an abstraction like and interface type) to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection AddType<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<TService, TImplementation>();
                break;

            case ServiceLifetime.Singleton:
                services.AddSingleton<TService, TImplementation>();
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<TService, TImplementation>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="TService"/> as the given lifetime using the factory specified in <paramref name="implementationFactory"/> for the type
    /// specified in <typeparamref name="TImplementation"/> into the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="TService">The type of the service (usually an abstraction like and interface type) to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection AddType<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime, Func<IServiceProvider, TImplementation> implementationFactory)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<TService>(implementationFactory);
                break;

            case ServiceLifetime.Singleton:
                services.AddSingleton<TService>(implementationFactory);
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<TService>(implementationFactory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="T"/> as the given lifetime with an instance specified in <paramref name="implementationInstance"/> to the collection
    /// of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationInstance">The instance of the service to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection AddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime, T implementationInstance)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(sp => implementationInstance);
                break;

            case ServiceLifetime.Singleton:
                services.AddSingleton(implementationInstance);
                break;

            case ServiceLifetime.Transient:
                services.AddTransient(sp => implementationInstance);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="T"/> as the given lifetime using the given factory specified in <paramref name="implementationFactory"/> into the
    /// collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection AddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime, Func<IServiceProvider, T> implementationFactory)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(implementationFactory);
                break;

            case ServiceLifetime.Singleton:
                services.AddSingleton(implementationFactory);
                break;

            case ServiceLifetime.Transient:
                services.AddTransient(implementationFactory);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Adds the specified <typeparamref name="T"/> as the given lifetime to the collection of services, if the service type hasn't already been registered.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the type to add.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If an invalid or unsupported value of <see cref="ServiceLifetime"/> is provided in parameter <paramref name="serviceLifetime"/>.
    /// </exception>
    public static IServiceCollection AddType<T>(this IServiceCollection services, ServiceLifetime serviceLifetime)
        where T : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<T>();
                break;

            case ServiceLifetime.Singleton:
                services.AddSingleton<T>();
                break;

            case ServiceLifetime.Transient:
                services.AddTransient<T>();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime));
        }

        return services;
    }
}
