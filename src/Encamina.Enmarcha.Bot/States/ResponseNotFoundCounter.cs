namespace Encamina.Enmarcha.Bot.States;

/// <summary>
/// Keeps track of how many response has the bot failed so far.
/// </summary>
public class ResponseNotFoundCounter
{
    /// <summary>
    /// Gets or sets number of not found responses.
    /// </summary>
    public int ResponseNotFound { get; set; }
}
