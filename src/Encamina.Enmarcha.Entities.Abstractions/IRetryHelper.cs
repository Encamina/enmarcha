namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Helper for retrying failed operations.
/// </summary>
public interface IRetryHelper
{
    /// <summary>
    /// Retry an asynchronous operation a specified number of times.
    /// </summary>
    /// <param name="retryTimes">Number of times to retry the operation.</param>
    /// <param name="waitTimeMilliseconds">Delay between each retry attempt (in milliseconds).</param>
    /// <param name="operation">Asynchronous operation to be executed and retried if it fails.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RetryOperationAsync(int retryTimes, int waitTimeMilliseconds, Func<Task> operation);
}
