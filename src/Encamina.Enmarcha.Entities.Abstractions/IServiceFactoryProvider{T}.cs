using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents a provider for factories of services of type '<typeparamref name="T"/>'.
/// </summary>
/// <typeparam name="T">The type for the service factories from this provider.</typeparam>
public interface IServiceFactoryProvider<out T> where T : class
{
    /// <summary>
    /// Gets a factory to create instances of <see cref="IServiceScope"/>, which is used to create services
    /// of type '<typeparamref name="T"/>' within a scope.
    /// </summary>
    IServiceScopeFactory ServiceScopeFactory { get; }

    /// <summary>
    /// Gets an instance of <see cref="IServiceFactory{T}"/> to create services of type '<typeparamref name="T"/>' within a scope,
    /// usually obtined from <see cref="ServiceScopeFactory"/>.
    /// </summary>
    /// <remarks>
    /// <b>IMPORTANT</b>: returned <see cref="IServiceFactory{T}"/> types as disposable. Please dispose them after use.
    /// </remarks>
    /// <returns>A valid instance of <see cref="IServiceFactory{T}"/>.</returns>
    IServiceFactory<T> GetScopedServiceFactory();
}
