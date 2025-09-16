namespace Encamina.Enmarcha.Agents.States;

/// <summary>
/// Keeps track of how many response has the agent failed so far.
/// </summary>
public class ResponseNotFoundCounter
{
    /// <summary>
    /// Gets or sets number of not found responses.
    /// </summary>
    public int ResponseNotFound { get; set; }
}
