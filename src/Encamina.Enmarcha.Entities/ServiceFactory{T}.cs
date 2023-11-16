using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Entities;

/// <summary>
/// A factory that can provide valid instances of a specific service of type '<typeparamref name="T"/>' within a scope.
/// </summary>
/// <typeparam name="T">The type of service this factory creates.</typeparam>
public class ServiceFactory<T> : IServiceFactory<T> where T : class
{
    private readonly IServiceScope serviceScope;
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceFactory{T}"/> class.
    /// </summary>
    /// <param name="serviceScope">A service scope from which to get service of type '<typeparamref name="T"/>'.</param>
    public ServiceFactory(IServiceScope serviceScope)
    {
        Guard.IsNotNull(serviceScope);

        this.serviceScope = serviceScope;
    }

    /// <inheritdoc/>
    public virtual T ById<TId>(TId serviceId) => ById(serviceId, true);

    /// <inheritdoc/>
    public virtual T ById<TId>(TId serviceId, bool throwIfNotFound)
    {
        return ProcessService(GetService<T>(s => s is IIdentifiable<TId> identifiable && identifiable.Id.Equals(serviceId)), throwIfNotFound, Resources.ExceptionMessages.InvalidServiceId, nameof(serviceId), typeof(T), serviceId);
    }

    /// <inheritdoc/>
    public virtual T ByName(string serviceName) => ByName(serviceName, true);

    /// <inheritdoc/>
    public virtual T ByName(string serviceName, bool throwIfNotFound)
    {
        return ProcessService(GetService<T>(s => s is INameable nameable && nameable.Name.Equals(serviceName)), throwIfNotFound, Resources.ExceptionMessages.InvalidServiceName, nameof(serviceName), typeof(T), serviceName);
    }

    /// <inheritdoc/>
    public virtual T ByType(Type serviceType) => ByType(serviceType, true);

    /// <inheritdoc/>
    public virtual T ByType(Type serviceType, bool throwIfNotFound)
    {
        return ProcessService(GetService<T>(s => s.GetType() == serviceType), throwIfNotFound, Resources.ExceptionMessages.InvaidServiceType, nameof(serviceType), serviceType);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes this instances and its related resources.
    /// </summary>
    /// <remarks>
    /// This method disposes the <see cref="IServiceScope"/> used by this factory.
    /// </remarks>
    /// <param name="disposing">
    /// A flag value indicating whether this instance is being disponsed or not, to prevent redundant calls.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                serviceScope.Dispose();
            }

            disposed = true;
        }
    }

    private static T ProcessService(T service, bool throwIfNotFound, string exceptionMessage, string parameterName, params object[] exceptionMessageParameters)
    {
        return service == null && throwIfNotFound
            ? throw new ArgumentException(string.Format(exceptionMessage, exceptionMessageParameters), parameterName)
            : service;
    }

    private TService GetService<TService>(Func<TService, bool> filter) where TService : class
    {
        return serviceScope.ServiceProvider.GetServices<TService>().FirstOrDefault(filter);
    }
}
