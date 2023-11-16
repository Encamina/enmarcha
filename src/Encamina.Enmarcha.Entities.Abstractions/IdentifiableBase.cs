using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Base class for entities that must have a property that represents a unique identifier.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class IdentifiableBase : IIdentifiable
{
    /// <inheritdoc/>
    public object Id { get; init; }
}
