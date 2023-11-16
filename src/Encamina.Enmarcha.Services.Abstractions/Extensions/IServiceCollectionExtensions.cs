using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection.Extensions;

using ExceptionMessages = Encamina.Enmarcha.Services.Abstractions.Resources.ExceptionMessages;

#pragma warning disable S103 // Lines should not be too long

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to add services related to execution contexts.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for custom execution contexts types.
    /// </summary>
    /// <remarks>Execution context are always added as scoped services.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="executionContextServiceType">The type of the execution context service (usually an abstraction like and interface type).</param>
    /// <param name="executionContextImplementationType">The type of the execution context implementation.</param>
    /// <param name="executionContextTemplateServiceType">The type of the execution context template service (usually an abstraction like and interface type).</param>
    /// <param name="executionContextTemplateImplementationType">The type of the execution context template implementation.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="services"/> or any of the other given parameters is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="executionContextServiceType"/> is not an interface type or if it does not inherits from <see cref="IExecutionContext"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="executionContextImplementationType"/> is not an interface type or if it does not inherits from <see cref="IExecutionContextTemplate"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="executionContextImplementationType"/> does not implements the interface <paramref name="executionContextServiceType"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="executionContextTemplateImplementationType"/> does not implements the interface <paramref name="executionContextTemplateServiceType"/>.</exception>
    /// <exception cref="ArgumentException">
    /// If <paramref name="executionContextImplementationType"/> does not accepts an instance of <paramref name="executionContextTemplateImplementationType"/> as parameter in any of its constructors.
    /// </exception>
    public static IServiceCollection AddExecutionContext(this IServiceCollection services, Type executionContextServiceType, Type executionContextImplementationType, Type executionContextTemplateServiceType, Type executionContextTemplateImplementationType)
    {
        Guard.IsNotNull(executionContextServiceType);
        Guard.IsNotNull(executionContextImplementationType);
        Guard.IsNotNull(executionContextTemplateServiceType);
        Guard.IsNotNull(executionContextTemplateImplementationType);

        // Validate if the implementation type has the service type among its interfaces.
        // We use name comparisson instead of calling `IsAssignableFrom` because sometimes
        // the Type instances might be the same yet not with all the properties informed for the
        // validation to succeed correctly.

        var typeOfExecutionContextTemplateIntr = typeof(IExecutionContextTemplate);
        var typeOfExecutionContextIntr = executionContextServiceType.IsGenericTypeDefinition ? typeof(IExecutionContext<>) : typeof(IExecutionContext);

        if (!executionContextServiceType.IsInterface)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.TypeIsNotInterface)), executionContextServiceType.FullName), nameof(executionContextServiceType));
        }

        if (!executionContextTemplateServiceType.IsInterface)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.TypeIsNotInterface)), executionContextTemplateServiceType.FullName), nameof(executionContextTemplateServiceType));
        }

        if (executionContextServiceType.Name != typeOfExecutionContextIntr.Name && executionContextServiceType.GetInterface(typeOfExecutionContextIntr.Name) == null)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidTypesInheritance)), executionContextServiceType.FullName, typeOfExecutionContextIntr.FullName), nameof(executionContextServiceType));
        }

        if (executionContextTemplateServiceType.Name != typeOfExecutionContextTemplateIntr.Name && executionContextTemplateServiceType.GetInterface(typeOfExecutionContextTemplateIntr.Name) == null)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidTypesInheritance)), executionContextTemplateServiceType.FullName, typeOfExecutionContextTemplateIntr.FullName), nameof(executionContextTemplateServiceType));
        }

        if (executionContextImplementationType.GetInterface(executionContextServiceType.Name) == null)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidTypesInheritance)), executionContextImplementationType.FullName, executionContextServiceType.FullName), nameof(executionContextImplementationType));
        }

        if (executionContextTemplateImplementationType.GetInterface(executionContextTemplateServiceType.Name) == null)
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidTypesInheritance)), executionContextTemplateImplementationType.FullName, executionContextTemplateServiceType.FullName), nameof(executionContextTemplateImplementationType));
        }

        // Finally, validate the execution context implementation type accepts an instance
        // of the given execution context template implementation as parameter in any of
        // its constructors.

        if (!executionContextImplementationType.GetConstructors().SelectMany(c => c.GetParameters()).Any(p => executionContextTemplateServiceType.IsAssignableFrom(p.ParameterType)))
        {
            throw new ArgumentException(string.Format(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.InvalidTemplateTypeForConstructor)), executionContextTemplateServiceType.FullName, executionContextImplementationType.FullName), nameof(executionContextImplementationType));
        }

        services.TryAddScoped(typeOfExecutionContextTemplateIntr, executionContextTemplateImplementationType);
        services.TryAddScoped(typeOfExecutionContextIntr, executionContextImplementationType);

        services.TryAddScoped(executionContextTemplateServiceType, executionContextTemplateImplementationType);
        services.TryAddScoped(executionContextServiceType, executionContextImplementationType);

        services.TryAddScoped(executionContextTemplateImplementationType);
        services.TryAddScoped(executionContextImplementationType);

        return services;
    }

    /// <summary>
    /// Adds support for custom implementations of the execution context and the execution context template.
    /// </summary>
    /// <typeparam name="TExecutionContextImpl">The type of the execution context implementation. Must implement the interface <see cref="IExecutionContext"/>.</typeparam>
    /// <typeparam name="TExecutionContextTemplateImpl">The type of the execution context template implementation. Must implement the interface <see cref="IExecutionContextTemplate"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="services"/> is <see langword="null"/>.</exception>
    public static IServiceCollection AddExecutionContext<TExecutionContextImpl, TExecutionContextTemplateImpl>(this IServiceCollection services)
        where TExecutionContextImpl : class, IExecutionContext, new()
        where TExecutionContextTemplateImpl : class, IExecutionContextTemplate, new()
    {
        services.TryAddScoped<IExecutionContextTemplate, TExecutionContextTemplateImpl>();
        services.TryAddScoped<IExecutionContext, TExecutionContextImpl>();

        services.TryAddScoped<TExecutionContextTemplateImpl>();
        services.TryAddScoped<TExecutionContextImpl>();

        return services;
    }

    /// <summary>
    /// Adds support for custom execution context and execution context template.
    /// </summary>
    /// <typeparam name="TExecutionContext">The type of the execution context service (usually an abstraction like and interface type). Must inherit from the interface <see cref="IExecutionContext"/>.</typeparam>
    /// <typeparam name="TExecutionContextImpl">The type of the execution context implementation. Must implement the interface defined by <typeparamref name="TExecutionContext"/>.</typeparam>
    /// <typeparam name="TExecutionContextTemplate">The type of the execution context template service (usually an abstraction like and interface type). Must inherit from the interface <see cref="IExecutionContextTemplate"/>.</typeparam>
    /// <typeparam name="TExecutionContextTemplateImpl">The type of the execution context template implementation. Must implement the interface defined by <typeparamref name="TExecutionContextTemplate"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="services"/> is <see langword="null"/>.</exception>
    [SuppressMessage("SonarLint", "S2436", Justification = "Four types are required to properly add an Execution Context.")]
    public static IServiceCollection AddExecutionContext<TExecutionContext, TExecutionContextImpl, TExecutionContextTemplate, TExecutionContextTemplateImpl>(this IServiceCollection services)
        where TExecutionContext : IExecutionContext
        where TExecutionContextImpl : class, TExecutionContext, new()
        where TExecutionContextTemplate : IExecutionContextTemplate
        where TExecutionContextTemplateImpl : class, TExecutionContextTemplate, new()
    {
        return services.AddExecutionContext(typeof(TExecutionContext), typeof(TExecutionContextImpl), typeof(TExecutionContextTemplate), typeof(TExecutionContextTemplateImpl));
    }
}

#pragma warning restore S103 // Lines should not be too long
