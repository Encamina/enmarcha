namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents a generic uniquely identifiable entity.
/// </summary>
/// <typeparam name="T">The type for the unique identifier of this entity.</typeparam>
public interface IIdentifiable<out T> : IIdentifiable
{
    /// <summary>
    /// Gets the uniquely identifier of this entity.
    /// </summary>
    new T Id { get; }
}
