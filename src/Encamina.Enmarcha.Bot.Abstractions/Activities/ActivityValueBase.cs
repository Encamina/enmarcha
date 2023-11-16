namespace Encamina.Enmarcha.Bot.Abstractions.Activities;

/// <summary>
/// Base class for any entity that could be send or received as an activity value.
/// </summary>
public class ActivityValueBase
{
    /// <summary>
    /// Gets the activity value type.
    /// </summary>
    public string Type { get; init; }
}
