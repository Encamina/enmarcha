namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents a uniquely identifiable entity.
/// </summary>
public interface IIdentifiable
{
    /// <summary>
    /// Gets the uniquely identifier of this entity.
    /// </summary>
    object Id { get; }
}
