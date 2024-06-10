using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Base class for a handlers' manager that uses handlers that implements the <see cref="INameable"/> interface.
/// </summary>
/// <typeparam name="THandler">The type of handlers to manage. Must implement the <see cref="INameable"/> interface.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class NameableHandlerManagerBase<THandler> : HandlerManagerBase<THandler>
    where THandler : INameable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NameableHandlerManagerBase{THandler}"/> class.
    /// </summary>
    /// <param name="handlers">A collection of handlers for this manager.</param>
    protected NameableHandlerManagerBase(IEnumerable<THandler> handlers)
    {
        Handlers = handlers?.ToDictionary(h => h.Name);
    }

    /// <summary>
    /// Gets the current collection of available handlers indexed by the handle's name.
    /// </summary>
    public new IDictionary<string, THandler>? Handlers { get; init; }
}
