using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Base class for a handlers' manager.
/// </summary>
/// <typeparam name="THandler">The type of handlers to manage.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class HandlerManagerBase<THandler>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerManagerBase{THandler}"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is required to ensure that order most specific handler managers,
    /// like <see cref="NameableHandlerManagerBase{THandler}"/> or <see cref="OrderableHandlerManagerBase{THandler}"/>
    /// can modify the nature of the <see cref="Handlers"/> property.
    /// </remarks>
    protected HandlerManagerBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerManagerBase{THandler}"/> class with
    /// a give collection of handlers.
    /// </summary>
    /// <param name="handlers">A collection of handlers for this manager.</param>
    protected HandlerManagerBase(IEnumerable<THandler> handlers)
    {
        Handlers = handlers;
    }

    /// <summary>
    /// Gets the current collection of available handlers.
    /// </summary>
    public virtual IEnumerable<THandler> Handlers { get; init; }
}
