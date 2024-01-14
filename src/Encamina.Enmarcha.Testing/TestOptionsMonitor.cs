using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Testing;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public sealed class TestOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
{
    private Action<TOptions, string> currentListener;

    /// <summary>
    /// 
    /// </summary>
    public TestOptionsMonitor()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentValue"></param>
    public TestOptionsMonitor(TOptions currentValue)
    {
        CurrentValue = currentValue;
    }

    /// <inheritdoc/>
    public TOptions CurrentValue { get; private set; }

    /// <inheritdoc/>
    public TOptions Get(string name) => CurrentValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
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