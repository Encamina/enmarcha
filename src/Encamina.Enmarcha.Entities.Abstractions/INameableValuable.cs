namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents entities with name and value.
/// </summary>
/// <remarks>
/// This is an alternative to <see cref="KeyValuePair"/> and <see cref="KeyValuePair{TKey, TValue}"/> which do not allow inheritance because
/// the former is an static class and the latter is an struct.
/// </remarks>
/// <typeparam name="T">The type for the value of this entity.</typeparam>
public interface INameableValuable<out T> : INameable, IValuable<T>
{
}
