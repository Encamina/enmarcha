namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents entities with name and unique identifier.
/// </summary>
/// <typeparam name="T">The type for the unique identifier of this entity.</typeparam>
public interface INameableIdentifiable<out T> : INameable, IIdentifiable<T>
{
}
