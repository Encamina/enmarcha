namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents an intendable entity, in other words, and entity that has an intent.
/// </summary>
public interface IIntendable
{
    /// <summary>
    /// Gets the intent of this entity.
    /// </summary>
    string Intent { get; }
}
