namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents a factory that can provide valid instances of a specific cognitive service type.
/// </summary>
/// <typeparam name="TCognitiveService">The type of specific cognitive service that this factory provides.</typeparam>
public interface ICognitiveServiceFactory<out TCognitiveService>
    where TCognitiveService : class, ICognitiveService
{
    /// <summary>
    /// Gets the specific cognitive service by its given name.
    /// </summary>
    /// <param name="cognitiveServiceName">The cognitive service name.</param>
    /// <returns>A valid instances of <typeparamref name="TCognitiveService"/> found by its name.</returns>
    TCognitiveService GetByName(string cognitiveServiceName);

    /// <summary>
    /// Gets the specific cognitive service by its given name.
    /// </summary>
    /// <param name="cognitiveServiceName">The cognitive service name.</param>
    /// <param name="throwIfNotFound">A flag indicating whether an exception is thrown if the cognitive service is not found.</param>
    /// <returns>A valid instances of <typeparamref name="TCognitiveService"/> found by its name.</returns>
    TCognitiveService? GetByName(string cognitiveServiceName, bool throwIfNotFound);
}

