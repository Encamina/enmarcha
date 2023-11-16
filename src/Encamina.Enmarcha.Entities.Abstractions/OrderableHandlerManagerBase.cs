using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Base class for a handlers' manager that uses handlers that implements the <see cref="IOrderable"/> interface.
/// </summary>
/// <typeparam name="THandler">The type of handlers to manage. Must implement the <see cref="IOrderable"/> interface.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class OrderableHandlerManagerBase<THandler> : HandlerManagerBase<THandler>
    where THandler : IOrderable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderableHandlerManagerBase{THandler}"/> class.
    /// </summary>
    /// <param name="handlers">A collection of handlers for this manager.</param>
    protected OrderableHandlerManagerBase(IEnumerable<THandler> handlers)
    {
        Handlers = handlers?.OrderBy(h => h.Order).ToList();
    }
}
