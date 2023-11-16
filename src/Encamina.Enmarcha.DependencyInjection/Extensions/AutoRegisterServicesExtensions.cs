using System.ComponentModel;
using System.Reflection;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Core.Extensions;

using Encamina.Enmarcha.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for auto-register services into an <see cref="IServiceCollection"/>.
/// </summary>
public static class AutoRegisterServicesExtensions
{
    /// <summary>
    /// Adds types decorated with the <see cref="AutoRegisterServiceAttribute"/> into the service collection from the assembly of the given type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A type to get the assembly from.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the autoregistered services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAutoRegisterServicesFromAssembly<T>(this IServiceCollection services)
    {
        InternalAddAutoRegisterServicesFromAssembly(services, typeof(T).Assembly);
        return services;
    }

    /// <summary>
    /// Adds types decorated with the <see cref="AutoRegisterServiceAttribute"/> into the service collection from the given assembly.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the autoregistered services to.</param>
    /// <param name="assembly">The assembly with autoregistered service types.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAutoRegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        InternalAddAutoRegisterServicesFromAssembly(services, assembly);
        return services;
    }

    private static void HandleScoped(IServiceCollection services, ISet<Type> interfaces, Type serviceType, bool forcedOnlyAsImplementationType, IEnumerable<Type> alternativeTypes)
    {
        services.AddScoped(serviceType);

        if (alternativeTypes?.Any() ?? false)
        {
            foreach (var alternativeType in alternativeTypes)
            {
                services.AddScoped(alternativeType, serviceType);
            }
        }

        if (!forcedOnlyAsImplementationType)
        {
            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, serviceType);
            }
        }
    }

    private static void HandleSingleton(IServiceCollection services, ISet<Type> interfaces, Type serviceType, bool forcedOnlyAsImplementationType, IEnumerable<Type> alternativeTypes)
    {
        services.AddSingleton(serviceType);

        if (alternativeTypes?.Any() ?? false)
        {
            foreach (var alternativeType in alternativeTypes)
            {
                services.AddSingleton(alternativeType, serviceType);
            }
        }

        if (!forcedOnlyAsImplementationType)
        {
            foreach (var @interface in interfaces)
            {
                services.AddSingleton(@interface, serviceType);
            }
        }
    }

    private static void HandleTransient(IServiceCollection services, ISet<Type> interfaces, Type serviceType, bool forcedOnlyAsImplementationType, IEnumerable<Type> alternativeTypes)
    {
        services.AddTransient(serviceType);

        if (alternativeTypes?.Any() ?? false)
        {
            foreach (var alternativeType in alternativeTypes)
            {
                services.AddTransient(alternativeType, serviceType);
            }
        }

        if (!forcedOnlyAsImplementationType)
        {
            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, serviceType);
            }
        }
    }

    private static void InternalAddAutoRegisterServicesFromAssembly(IServiceCollection services, Assembly assembly)
    {
        Guard.IsNotNull(services);
        Guard.IsNotNull(assembly);

        foreach (var serviceType in assembly.GetTypes().Where(t => t.IsClass))
        {
            var autoRegisterServiceAttribute = serviceType.GetCustomAttribute<AutoRegisterServiceAttribute>();

            if (autoRegisterServiceAttribute != null)
            {
                var implementedInterfaces = serviceType.GetInterfaces().Where(x => autoRegisterServiceAttribute.RegisterInterfaces).ToHashSet();

                RemoveInterfacesImplementedByOtherInterfaces(implementedInterfaces);

                if (!autoRegisterServiceAttribute.IncludeInheritedInterfaces && serviceType.BaseType != null)
                {
                    serviceType.BaseType.GetInterfaces().ToList().ForEach(x => implementedInterfaces.RemoveIfExists(x));
                }

                RemoveInterfacesUsedInAnyConstructor(serviceType, implementedInterfaces);

                switch (autoRegisterServiceAttribute.ServiceLifetime)
                {
                    case ServiceLifetime.Singleton:
                        HandleSingleton(services, implementedInterfaces, serviceType, autoRegisterServiceAttribute.ForceOnlyAsImplementationType, autoRegisterServiceAttribute.AlternativeTypes);
                        break;

                    case ServiceLifetime.Scoped:
                        HandleScoped(services, implementedInterfaces, serviceType, autoRegisterServiceAttribute.ForceOnlyAsImplementationType, autoRegisterServiceAttribute.AlternativeTypes);
                        break;

                    case ServiceLifetime.Transient:
                        HandleTransient(services, implementedInterfaces, serviceType, autoRegisterServiceAttribute.ForceOnlyAsImplementationType, autoRegisterServiceAttribute.AlternativeTypes);
                        break;

                    default:
                        throw new InvalidEnumArgumentException(nameof(autoRegisterServiceAttribute.ServiceLifetime), (int)autoRegisterServiceAttribute.ServiceLifetime, typeof(ServiceLifetime));
                }
            }
        }
    }

    private static void RemoveInterfacesImplementedByOtherInterfaces(ISet<Type> implementedInterfaces)
    {
        foreach (var implementedInterface in implementedInterfaces)
        {
            var subInterfaces = implementedInterface.GetInterfaces();

            if (subInterfaces.Length > 0)
            {
                foreach (var subInterface in subInterfaces)
                {
                    implementedInterfaces.RemoveIfExists(subInterface);
                }
            }
        }
    }

    private static void RemoveInterfacesUsedInAnyConstructor(Type type, ICollection<Type> implementedInterfaces)
    {
        foreach (var parameter in type.GetConstructors().SelectMany(c => c.GetParameters()).Where(p => implementedInterfaces.Contains(p.ParameterType)))
        {
            implementedInterfaces.Remove(parameter.ParameterType);
        }
    }
}
