namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents an entity with value.
/// </summary>
/// <typeparam name="T">The type for the value of this entity.</typeparam>
public interface IValuable<out T>
{
    /// <summary>
    /// Gets the value of this entity.
    /// </summary>
    public T Value { get; }
}
