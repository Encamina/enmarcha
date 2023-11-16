using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Entities;

/// <summary>
/// A provider for factories of services of type '<typeparamref name="T"/>'.
/// </summary>
/// <typeparam name="T">The type for the service factories from this provider.</typeparam>
public class ServiceFactoryProvider<T> : IServiceFactoryProvider<T> where T : class
{
    private readonly Func<IServiceScopeFactory, IServiceFactory<T>> serviceFactoryBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactoryProvider{T}"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">
    /// A factory to create instances of <see cref="IServiceScope"/>s, required to create services of type '<typeparamref name="T"/>' within a scope.
    /// </param>
    public ServiceFactoryProvider(IServiceScopeFactory serviceScopeFactory) : this(serviceScopeFactory, ssf => new ServiceFactory<T>(ssf.CreateScope()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactoryProvider{T}"/> class, using a specific <see cref="IServiceFactory{T}"/> factory builder.
    /// </summary>
    /// <param name="serviceScopeFactory">
    /// A factory to create instances of <see cref="IServiceScope"/>, required to create services of type '<typeparamref name="T"/>' within a scope.
    /// </param>
    /// <param name="serviceFactoryBuilder">A builder for <see cref="IServiceFactory{T}"/> factories.</param>
    protected ServiceFactoryProvider(IServiceScopeFactory serviceScopeFactory, Func<IServiceScopeFactory, IServiceFactory<T>> serviceFactoryBuilder)
    {
        Guard.IsNotNull(serviceScopeFactory);
        Guard.IsNotNull(serviceFactoryBuilder);

        ServiceScopeFactory = serviceScopeFactory;
        this.serviceFactoryBuilder = serviceFactoryBuilder;
    }

    /// <inheritdoc/>
    public virtual IServiceScopeFactory ServiceScopeFactory { get; }

    /// <inheritdoc/>
    public virtual IServiceFactory<T> GetScopedServiceFactory() => serviceFactoryBuilder(ServiceScopeFactory);
}
