using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Testing;

/// <summary>
/// Represents an implementation of the <see cref="IOptionsMonitor{TOptions}"/> interface for testing purposes.
/// </summary>
/// <typeparam name="TOptions">The type of options being monitored.</typeparam>
public sealed class TestOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
{
    private Action<TOptions, string> currentListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestOptionsMonitor{TOptions}"/> class.
    /// </summary>
    public TestOptionsMonitor()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestOptionsMonitor{TOptions}"/> class with the specified initial value.
    /// </summary>
    /// <param name="currentValue">The initial value of the options.</param>
    public TestOptionsMonitor(TOptions currentValue)
    {
        CurrentValue = currentValue;
    }

    /// <inheritdoc/>
    public TOptions CurrentValue { get; private set; }

    /// <inheritdoc/>
    public TOptions Get(string name) => CurrentValue;

    /// <summary>
    /// Sets the current value of the options and invokes the change listener if registered.
    /// </summary>
    /// <param name="value">The new value of the options.</param>
    public void Set(TOptions value)
    {
        CurrentValue = value;
        currentListener?.Invoke(value, string.Empty);
    }

    /// <inheritdoc/>
    public IDisposable OnChange(Action<TOptions, string> listener)
    {
        this.currentListener = listener;
        return null;
    }
}