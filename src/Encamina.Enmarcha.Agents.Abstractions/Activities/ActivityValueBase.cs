namespace Encamina.Enmarcha.Agents.Abstractions.Activities;

/// <summary>
/// Base class for any entity that could be sent or received as an activity value.
/// </summary>
public class ActivityValueBase
{
    /// <summary>
    /// Gets the activity value type.
    /// </summary>
    public required string Type { get; init; }
}
