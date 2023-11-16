namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents entities with unique identifier and value.
/// </summary>
/// <typeparam name="TId">The type for the unique identifier of this entity.</typeparam>
/// <typeparam name="TValue">The type for the value of this entity.</typeparam>
public interface IIdentifiableValuable<out TId, out TValue> : IIdentifiable<TId>, IValuable<TValue>
{
}
