using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents a collection of functions to work chat history.
/// </summary>
public interface IChatHistoryProvider
{
    /// <summary>
    /// Deletes all chat history messages for a specific user.
    /// </summary>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    Task DeleteChatMessagesHistoryAsync(string indexerId, CancellationToken cancellationToken);

    /// <summary>
    /// Loads chat history messages.
    /// </summary>
    /// <param name="chatHistory">The current chat history.</param>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="remainingTokens">The total remaining tokens available for loading messages from the chat history.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    Task LoadChatMessagesHistoryAsync(ChatHistory chatHistory, string indexerId, int remainingTokens, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a chat message into the conversation history.
    /// </summary>
    /// <param name="indexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="roleName">The name of the role associated with the chat message. For example the `user`, the `assistant` or the `system`.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    Task SaveChatMessagesHistoryAsync(string indexerId, string roleName, string message, CancellationToken cancellationToken);
}
