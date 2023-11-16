namespace Encamina.Enmarcha.Bot.Logging;

/// <summary>
/// An empty scope without any logic.
/// </summary>
internal class NullScope : IDisposable
{
    private NullScope()
    {
    }

    /// <summary>
    /// Gets the instance of the <see cref="NullScope"/> class.
    /// </summary>
    public static NullScope Instance { get; } = new NullScope();

    /// <inheritdoc />
    public void Dispose()
    {
        // Nothing to do here.
    }
}
