// Ignore Spelling: Debouncer

namespace Encamina.Enmarcha.Core;

/// <summary>
/// Provides mechanisms to prevent multiple bouncing calls to a method or event.
/// </summary>
public static class Debouncer
{
    /// <summary>
    /// Debounce a function from being called too frequently. This is often used in scenarios where you want to limit the rate of execution of a particular method or event.
    /// </summary>
    /// <typeparam name="T">The type of the argument to be passed to the function to be debounced.</typeparam>
    /// <param name="function">The function to be debounced.</param>
    /// <param name="milliseconds">The number of milliseconds to wait before calling the function again.</param>
    /// <returns>A function that can be called to debounce the original function.</returns>
    public static Action<T> Debounce<T>(Action<T> function, int milliseconds = 300)
    {
        var last = 0;

        return arg =>
        {
            var current = Interlocked.Increment(ref last);

            Task.Delay(milliseconds).ContinueWith(task =>
            {
                if (current == last)
                {
                    function(arg);
                }

                task.Dispose();
            });
        };
    }
}
