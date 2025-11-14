namespace Encamina.Enmarcha.Agents.Models;

/// <summary>
/// Represents the status of a live activity.
/// </summary>
public enum LiveActivityStatus
{
    /// <summary>
    /// The activity is currently running.
    /// </summary>
    Running = 0,

    /// <summary>
    /// The activity has completed successfully.
    /// </summary>
    Completed = 1,

    /// <summary>
    /// The activity has failed.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// The activity has completed with warnings.
    /// </summary>
    Warning = 3,
}
