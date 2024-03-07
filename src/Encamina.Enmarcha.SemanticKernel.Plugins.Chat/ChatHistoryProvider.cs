using Encamina.Enmarcha.Data.Abstractions;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat;

/// <inheritdoc/>
public class ChatHistoryProvider : IChatHistoryProvider
{
    private readonly IAsyncRepository<ChatMessageHistoryRecord> chatMessagesHistoryRepository;
    private readonly Func<string, int> tokensLengthFunction;

    private ChatHistoryProviderOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHistoryProvider"/> class.
    /// </summary>
    /// <param name="tokensLengthFunction">Function to calculate the length of a string (usually the chat messages) in tokens.</param>
    /// <param name="chatMessagesHistoryRepository">A valid instance of an asynchronous repository pattern implementation.</param>
    /// <param name="options">Configuration options for this provider.</param>
    public ChatHistoryProvider(Func<string, int> tokensLengthFunction, IAsyncRepository<ChatMessageHistoryRecord> chatMessagesHistoryRepository, IOptionsMonitor<ChatHistoryProviderOptions> options)
    {
        this.tokensLengthFunction = tokensLengthFunction;
        this.chatMessagesHistoryRepository = chatMessagesHistoryRepository;
        this.options = options.CurrentValue;

        options.OnChange((newOptions) => this.options = newOptions);
    }

    /// <inheritdoc/>
    public async Task DeleteChatMessagesHistoryAsync(string userId, CancellationToken cancellationToken)
    {
        await chatMessagesHistoryRepository.DeleteAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The maximum number of messages to load is configured in <c>ChatHistoryProviderOptions.HistoryMaxMessages</c>.
    /// </remarks>
    public async Task LoadChatMessagesHistoryAsync(ChatHistory chatHistory, string userId, int remainingTokens, CancellationToken cancellationToken)
    {
        if (options.HistoryMaxMessages <= 0 || remainingTokens <= 0)
        {
            return;
        }

        // Obtain the chat history for the user, ordered by timestamps descending to get the most recent messages first, and then take 'N' messages.
        // This means that the first elements in the list are the most recent or newer messages, and the last elements in the list are the oldest messages.
        var result = (await chatMessagesHistoryRepository.GetAllAsync(chatMessages => chatMessages.Where(chatMessage => chatMessage.UserId == userId)
                                                                                                  .OrderByDescending(chatMessage => chatMessage.TimestampUtc)
                                                                                                  .Take(options.HistoryMaxMessages), cancellationToken)).ToList();

        // Previous lines loads into `result` a list of chat history messages like this (U = stands for `User`; A = stands for `Assistant`, a message from the AI):
        // Newer Messages -------> Older Messages
        // A10 U10 A9 U9 A8 U8 A7 U7 A6 U6 A5 U5 A4 U4 A3 U3 A2 U2 A1 U1

        var assistantRoleName = AuthorRole.Assistant.ToString(); // Get this here to slightly improve performance...

        result.TakeWhile(item =>
        {
            var itemRole = item.RoleName == assistantRoleName ? AuthorRole.Assistant : AuthorRole.User;
            var tokensHistoryMessage = ILengthFunctions.LengthChatMessage(item.Message, itemRole, tokensLengthFunction);

            if (tokensHistoryMessage <= remainingTokens)
            {
                remainingTokens -= tokensHistoryMessage;
                return true;
            }

            return false;

            // The `TakeWhile` will reduce the number of chat history messages, taking the most recent messages until the remaining tokens are less than the tokens of the current message.
            // Newer Messages -------> Older Messages
            // A10 U10 A9 U9 A8 U8 A7 U7 A6 U6
        }).Reverse().ToList().ForEach(item =>
        {
            // The (previous) `Reverse` will invert the order of the messages, so the oldest messages are the first elements in the list.
            // This is required because the `ChatHistory` class stacks messages in the order they were received.
            // The oldest came first, and the newest came last in the stack of messages.
            // Older Messages -------> Newer Messages
            // U6 A6 U7 A7 U8 A8 U9 A9 U10 A10

            // In some scenarios, empty messages might be retrieved as `null` and serialized as such when creating the request message for the LLM resulting in an HTTP 400 error.
            // Prevent this by setting the message as an empty string if the message is `null`.
            var msg = item.Message ?? string.Empty;

            if (item.RoleName == assistantRoleName)
            {
                chatHistory.AddAssistantMessage(msg);
            }
            else
            {
                chatHistory.AddUserMessage(msg);
            }
        });
    }

    /// <inheritdoc/>
    public async Task SaveChatMessagesHistoryAsync(string userId, string roleName, string message, CancellationToken cancellationToken)
    {
        await chatMessagesHistoryRepository.AddAsync(new ChatMessageHistoryRecord()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            RoleName = roleName,
            Message = message,
            TimestampUtc = DateTime.UtcNow,
        }, cancellationToken);
    }
}
