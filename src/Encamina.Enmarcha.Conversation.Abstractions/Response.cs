using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Conversation.Abstractions;

/// <summary>
/// Represents a bot's response, usually obtained from a (static) repository like a database or table storage.
/// </summary>
public class Response : IOrderable
{
    /// <summary>
    /// Gets this response order or preference.
    /// </summary>
    public virtual int Order { get; init; }

    /// <summary>
    /// Gets this response text.
    /// </summary>
    public virtual string Text { get; init; }
}