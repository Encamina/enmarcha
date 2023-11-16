namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Reprensents entities with name, unique identifier and value.
/// </summary>
/// <typeparam name="TId">The type for the unique identifier of this entity.</typeparam>
/// <typeparam name="TValue">The type for the value of this entity.</typeparam>
public interface INameableIdentifiableValuable<out TId, out TValue> : INameable, IIdentifiable<TId>, IValuable<TValue>
{
}
