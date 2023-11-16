namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents a nameable entity.
/// </summary>
public interface INameable
{
    /// <summary>
    /// Gets the name of this entity, service, or type.
    /// </summary>
    string Name { get; }
}
