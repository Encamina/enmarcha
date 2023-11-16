using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Base class for entities that must have a property that represents a unique identifier with a specific type.
/// </summary>
/// <typeparam name="T">The specific type for the unique identifier.</typeparam>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class IdentifiableBase<T> : IIdentifiable<T>
{
    /// <inheritdoc/>
    public virtual T Id { get; init; }

    /// <inheritdoc/>
    object IIdentifiable.Id => Id;
}
