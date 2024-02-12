using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents a collection of functions to work chat history.
/// </summary>
public interface IChatHistoryProvider
{
    /// <summary>
    /// Loads chat history messages.
    /// </summary>
    /// <remarks>
    /// The maximum number of messages to load is configured in ChatHistoryProviderOptions.HistoryMaxMessages.
    /// </remarks>
    /// <param name="chatHistory">The current chat history.</param>
    /// <param name="userId">The unique identifier of the user that is owner of the chat.</param>
    /// <param name="remainingTokens">The total remaining tokens available for loading messages from the chat history.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    public Task LoadChatMessagesHistoryAsync(ChatHistory chatHistory, string userId, int remainingTokens, CancellationToken cancellationToken);

    /// <summary>
    /// Saves a chat message into the conversation history.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="roleName">The name of the role associated with the chat message. For example the `user`, the `assistant` or the `system`.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    public Task SaveChatMessagesHistoryAsync(string userId, string roleName, string message, CancellationToken cancellationToken);
}
