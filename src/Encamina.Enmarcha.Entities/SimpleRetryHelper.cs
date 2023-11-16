using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Entities;

/// <summary>
/// A simple implementation of a helper for retrying failed operations.
/// </summary>
public class SimpleRetryHelper : IRetryHelper
{
    private readonly ILogger<SimpleRetryHelper> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleRetryHelper"/> class.
    /// </summary>
    /// <param name="logger">A valid instance of <see cref="ILogger"/> to log messages.</param>
    public SimpleRetryHelper(ILogger<SimpleRetryHelper> logger)
    {
        this.logger = logger;
    }

    /// <inheritdoc/>
    public virtual async Task RetryOperationAsync(int retryTimes, int waitTimeMilliseconds, Func<Task> operation)
    {
        for (var i = 0; i <= retryTimes; i++)
        {
            try
            {
                await operation();
                return; // Operation successful...
            }
            catch (Exception exception)
            {
                if (i < retryTimes)
                {
                    logger.LogWarning(exception, $@"Retry {i + 1}. Exception message was: {exception.Message}");

                    await Task.Delay(waitTimeMilliseconds);
                }
                else
                {
                    throw; // Throw original exception if operation still fails after all retries...
                }
            }
        }
    }
}
