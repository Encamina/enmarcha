namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents a factory that can provide valid instances of a specific service of type '<typeparamref name="T"/>' within a scope.
/// </summary>
/// <typeparam name="T">The type of service this factory creates.</typeparam>
public interface IServiceFactory<out T> : IDisposable where T : class
{
    /// <summary>
    /// Gets a service by its unique identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the service unique identifier.</typeparam>
    /// <param name="serviceId">The service unique identifier.</param>
    /// <returns>A valid instance of the service.</returns>
    T ById<TId>(TId serviceId);

    /// <summary>
    /// Gets a service by its unique identifier, and optionally throws an exception if not found.
    /// </summary>
    /// <typeparam name="TId">The type of the service unique identifier.</typeparam>
    /// <param name="serviceId">The service unique identifier.</param>
    /// <param name="throwIfNotFound">
    /// If <see langword="true"/> and the service is not found by its unique identifier, then throw an exception;
    /// otherwise <see langword="false"/> and don't throw any exception.
    /// </param>
    /// <returns>A valid instance of the service if found.</returns>
    T? ById<TId>(TId serviceId, bool throwIfNotFound);

    /// <summary>
    /// Gets a service by its name.
    /// </summary>
    /// <param name="serviceName">The service name.</param>
    /// <returns>A valid instance of the service.</returns>
    T ByName(string serviceName);

    /// <summary>
    /// Gets a service by its name, and optionally throws an exception if not found.
    /// </summary>
    /// <param name="serviceName">The service name.</param>
    /// <param name="throwIfNotFound">
    /// If <see langword="true"/> and the service is not found by its name, then throw an exception;
    /// otherwise <see langword="false"/> and don't throw any exception.
    /// </param>
    /// <returns>A valid instance of the service if found.</returns>
    T? ByName(string serviceName, bool throwIfNotFound);

    /// <summary>
    /// Gets a service by its type.
    /// </summary>
    /// <param name="serviceType">
    /// The service type, not necessarily the same as <typeparamref name="T"/>, but an inherited type.
    /// </param>
    /// <returns>A valid instance of the service.</returns>
    T ByType(Type serviceType);

    /// <summary>
    /// Gets a service by its type, and optionally throws an exception if not found.
    /// </summary>
    /// <param name="serviceType">
    /// The service type, not necessarily the same as <typeparamref name="T"/>, but an inherited type.
    /// </param>
    /// <param name="throwIfNotFound">
    /// If <see langword="true"/> and the service is not found by its type, then throw an exception;
    /// otherwise <see langword="false"/> and don't throw any exception.
    /// </param>
    /// <returns>A valid instance of the service if found.</returns>
    T? ByType(Type serviceType, bool throwIfNotFound);
}
