using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents entities that provides their own validation mechanism.
/// </summary>
public interface IValidableEntity
{
    /// <summary>
    /// Validates the current entity instance. If the entity is not valid, a collection of validation error messages is returned.
    /// </summary>
    /// <returns>A collection of validation error messages if the entity instance is invalid.</returns>
    IEnumerable<string> Validate();

    /// <summary>
    /// Validates the current entity instance. If the entity is not valid, an exception (usually a <see cref="AggregateException"/> with <see cref="ValidationException"/>s inside) is thrown.
    /// </summary>
    void ValidateAndThrowException();
}
