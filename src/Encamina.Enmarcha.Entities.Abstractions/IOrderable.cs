namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents an orderable type.
/// </summary>
public interface IOrderable
{
    /// <summary>
    /// Gets the order of this type.
    /// </summary>
    int Order { get; }
}
