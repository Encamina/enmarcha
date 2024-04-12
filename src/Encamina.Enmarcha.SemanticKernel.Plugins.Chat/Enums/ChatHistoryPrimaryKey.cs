namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Enums;

/// <summary>
/// Represents the posible Partition Keys for the chat history.
/// </summary>
public enum ChatHistoryPrimaryKey
{
    /// <summary>
    /// The unique identifier of the conversation.
    /// </summary>
    ConversationId,

    /// <summary>
    /// The unique identifier of the user owner of the chat.
    /// </summary>
    UserId,
}
