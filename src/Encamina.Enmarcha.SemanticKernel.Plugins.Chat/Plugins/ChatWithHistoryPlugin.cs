using System.ComponentModel;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.Data.Abstractions;

using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

// TODO - Review the tokens count to be align with the chat model. Use as reference the code from the Chat Copilot project.

/// <summary>
/// Represents a plugin that allows users to interact while chatting and asking questions to an Artificial Intelligence, usually a Large Language Model (LLM).
/// </summary>
public class ChatWithHistoryPlugin
{
    private readonly string chatModelName;
    private readonly IAsyncRepository<ChatMessageHistoryRecord> chatMessagesHistoryRepository;
    private readonly Kernel kernel;
    private readonly Func<string, int> tokensLengthFunction;

    private ChatWithHistoryPluginOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatWithHistoryPlugin"/> class.
    /// </summary>
    /// <param name="kernel">The instance of the semantic kernel to work with in this plugin.</param>
    /// <param name="chatModelName">The name of the chat model used by this plugin.</param>
    /// <param name="tokensLengthFunction">Function to calculate the length of a string (usually the chat messages) in tokens.</param>
    /// <param name="chatMessagesHistoryRepository">A valid instance of an asynchronous repository pattern implementation.</param>
    /// <param name="options">Configuration options for this plugin.</param>
    public ChatWithHistoryPlugin(Kernel kernel, string chatModelName, Func<string, int> tokensLengthFunction, IAsyncRepository<ChatMessageHistoryRecord> chatMessagesHistoryRepository, IOptionsMonitor<ChatWithHistoryPluginOptions> options)
    {
        this.kernel = kernel;
        this.chatModelName = chatModelName;
        this.chatMessagesHistoryRepository = chatMessagesHistoryRepository;
        this.options = options.CurrentValue;
        this.tokensLengthFunction = tokensLengthFunction;

        options.OnChange((newOptions) => this.options = newOptions);
    }

    /// <summary>
    /// Gets the system prompt to be used by the <c>Chat</c> function.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/azure/ai-services/openai/concepts/advanced-prompt-engineering?pivots=programming-language-chat-completions#system-message"/>
    protected virtual string SystemPrompt { get; }
        = @$"This is a chat between an artificial intelligence (AI) and a user. You don't have the ability to access data on the Internet, so you shouldn't claim that you can or say you'll go looking for things. Try to be concise in your answers, although it is not required.";

    /// <summary>
    /// Allows users to chat and ask questions to an Artificial Intelligence.
    /// </summary>
    /// <param name="ask">What the user says or asks when chatting.</param>
    /// <param name="userId">A unique identifier for the user when chatting.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="locale">The preferred language of the user while chatting.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A string representing the response from the Artificial Intelligence.</returns>
    [KernelFunction]
    [Description(@"Allows users to chat and ask questions to an Artificial Intelligence.")]
    public virtual async Task<string> ChatAsync(
        [Description(@"What the user says or asks when chatting")] string ask,
        [Description(@"A unique identifier for the user when chatting")] string userId,
        [Description(@"The name of the user")] string userName,
        [Description(@"The preferred language of the user while chatting")] string locale,
        CancellationToken cancellationToken)
    {
        var systemPrompt = $@"{SystemPrompt} The name of the user is {userName}. The user prefers responses using the language identified as {locale}. Always answer using {locale} as language.";

        var chatModelMaxTokens = ModelInfo.GetById(chatModelName).MaxTokens;
        var askTokens = tokensLengthFunction(ask);
        var systemPromptTokens = tokensLengthFunction(systemPrompt);

        var remainingTokens = chatModelMaxTokens - askTokens - systemPromptTokens - (options.ChatRequestSettings.MaxTokens ?? 0);

        var chatHistory = new ChatHistory(systemPrompt);

        if (remainingTokens < 0)
        {
            return await GetErrorMessageAsync(chatHistory, locale, systemPromptTokens, askTokens, chatModelMaxTokens, cancellationToken);
        }

        await LoadChatHistoryAsync(chatHistory, userId, remainingTokens, cancellationToken);

        chatHistory.AddUserMessage(ask);

        var chatMessage = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(chatHistory, options.ChatRequestSettings, kernel, cancellationToken);
        var response = chatMessage.Content;

        await SaveChatMessagesHistory(userId, AuthorRole.User.ToString(), ask, cancellationToken);               // Save in chat history the user message (a.k.a. ask).
        await SaveChatMessagesHistory(userId, AuthorRole.Assistant.ToString(), response, cancellationToken);     // Save in chat history the assistant message (a.k.a. response).

        return response;
    }

    /// <summary>
    /// Sometimes the chat history is too long and the chat model cannot process it. For those cases, will create an error message for the user.
    /// </summary>
    /// <param name="chatHistory">The current chat history.</param>
    /// <param name="locale">The preferred language of the user while chatting.</param>
    /// <param name="systemPromptTokens">Total tokens of the system prompt.</param>
    /// <param name="askTokens">Total tokens from the «ask» message of the user.</param>
    /// <param name="chatModelTokens">Total tokens supported by the chat model.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The error message.</returns>
    protected virtual async Task<string> GetErrorMessageAsync(ChatHistory chatHistory, string locale, int systemPromptTokens, int askTokens, int chatModelTokens, CancellationToken cancellationToken)
    {
        var prompt = @$"Please translate from 'EN-us' to '{locale}' the following text: The length of the system prompt is {systemPromptTokens} and the length of the user ask is {askTokens}. The maximum length of the prompt and the user ask is {chatModelTokens}.";

        chatHistory.AddSystemMessage(prompt);

        var chatMessage = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(chatHistory, options.ChatRequestSettings, kernel, cancellationToken);

        return chatMessage.Content;
    }

    /// <summary>
    /// Loads chat history messages.
    /// </summary>
    /// <remarks>
    /// The maximum number of messages to load is configured in <see cref="ChatWithHistoryPluginOptions.HistoryMaxMessages"/>.
    /// </remarks>
    /// <param name="chatHistory">The current chat history.</param>
    /// <param name="userId">The unique identifier of the user that is owner of the chat.</param>
    /// <param name="remainingTokens">The total remaining tokens available for loading messages from the chat history.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    protected virtual async Task LoadChatHistoryAsync(ChatHistory chatHistory, string userId, int remainingTokens, CancellationToken cancellationToken)
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
            var tokensHistoryMessage = tokensLengthFunction(item.Message);

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

    /// <summary>
    /// Saves a chat message into the conversation history.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="roleName">The name of the role associated with the chat message. For example the `user`, the `assistant` or the `system`.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A <see cref="Task"/> that on completion indicates the asynchronous operation has executed.</returns>
    protected virtual async Task SaveChatMessagesHistory(string userId, string roleName, string message, CancellationToken cancellationToken)
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
