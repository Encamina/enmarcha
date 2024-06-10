using System.ComponentModel;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;

using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

/// <summary>
/// Represents a plugin that allows users to interact while chatting and asking questions to an Artificial Intelligence, usually a Large Language Model (LLM).
/// </summary>
public class ChatWithHistoryPlugin
{
    private readonly string chatModelName;
    private readonly IChatHistoryProvider chatHistoryProvider;
    private readonly Kernel kernel;
    private readonly Func<string, int> tokensLengthFunction;

    private ChatWithHistoryPluginOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatWithHistoryPlugin"/> class.
    /// </summary>
    /// <param name="kernel">The instance of the semantic kernel to work with in this plugin.</param>
    /// <param name="chatModelName">The name of the chat model used by this plugin.</param>
    /// <param name="tokensLengthFunction">Function to calculate the length of a string (usually the chat messages) in tokens.</param>
    /// <param name="chatHistoryProvider">A valid instance of a chat history provider.</param>
    /// <param name="options">Configuration options for this plugin.</param>
    public ChatWithHistoryPlugin(Kernel kernel, string chatModelName, Func<string, int> tokensLengthFunction, IChatHistoryProvider chatHistoryProvider, IOptionsMonitor<ChatWithHistoryPluginOptions> options)
    {
        this.kernel = kernel;
        this.chatModelName = chatModelName;
        this.chatHistoryProvider = chatHistoryProvider;
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
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <param name="ask">What the user says or asks when chatting.</param>
    /// <param name="chatIndexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="userName">The name of the user.</param>
    /// <param name="locale">The preferred language of the user while chatting.</param>
    /// <returns>A string representing the response from the Artificial Intelligence.</returns>
    [KernelFunction]
    [Description(@"Allows users to chat and ask questions to an Artificial Intelligence.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(@"Design", @"CA1068:CancellationToken parameters must come last", Justification = @"We want to have optional parameters")]
    public virtual async Task<string?> ChatAsync(
        CancellationToken cancellationToken,
        [Description(@"What the user says or asks when chatting")] string ask,
        [Description(@"The unique identifier of the chat history indexer")] string chatIndexerId,
        [Description(@"The name of the user")] string? userName = null,
        [Description(@"The preferred language of the user while chatting")] string? locale = null)
    {
        var systemPrompt = SystemPrompt;

        if (!string.IsNullOrWhiteSpace(userName))
        {
            systemPrompt += $@" The name of the user is {userName}.";
        }

        if (!string.IsNullOrWhiteSpace(locale))
        {
            systemPrompt += $@" The user prefers responses using the language identified as {locale}. Always answer using {locale} as language.";
        }

        var chatModelMaxTokens = ModelInfo.GetById(chatModelName).MaxTokens;
        var askTokens = ILengthFunctions.LengthChatMessage(ask, AuthorRole.User, tokensLengthFunction);
        var systemPromptTokens = ILengthFunctions.LengthChatMessage(systemPrompt, AuthorRole.System, tokensLengthFunction);

        var remainingTokens = chatModelMaxTokens - askTokens - systemPromptTokens - (options.ChatRequestSettings.MaxTokens ?? 0);

        var chatHistory = new ChatHistory(systemPrompt);

        if (remainingTokens < 0)
        {
            return await GetErrorMessageAsync(chatHistory, locale, systemPromptTokens, askTokens, chatModelMaxTokens, cancellationToken);
        }

        await chatHistoryProvider.LoadChatMessagesHistoryAsync(chatHistory, chatIndexerId, remainingTokens, cancellationToken);

        chatHistory.AddUserMessage(ask);

        var chatMessage = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(chatHistory, options.ChatRequestSettings, kernel, cancellationToken);
        var response = chatMessage.Content;

        var chatHistoryToSave = new List<ChatMessageContent>()
        {
            new(AuthorRole.User, ask), // Save in chat history the user message (a.k.a. ask).
            new(AuthorRole.Assistant, response), // Save in chat history the assistant message (a.k.a. response).
        };
        await chatHistoryProvider.SaveChatMessagesHistoryBatchAsync(chatIndexerId, chatHistoryToSave, cancellationToken);

        return response;
    }

    /// <summary>
    /// Deletes the chat message history when a user asks to forget previous conversations or to start over again.
    /// </summary>
    /// <param name="chatIndexerId">The unique identifier of the chat history indexer.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    [KernelFunction]
    [Description(@"Deletes the chat message history when a user asks to forget previous conversations or to start all over again.")]
    public async Task DeleteChatMessagesHistoryAsync(
        [Description(@"The unique identifier of the chat history indexer")] string chatIndexerId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chatIndexerId);

        await chatHistoryProvider.DeleteChatMessagesHistoryAsync(chatIndexerId, cancellationToken);
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
    protected virtual async Task<string?> GetErrorMessageAsync(ChatHistory chatHistory, string? locale, int systemPromptTokens, int askTokens, int chatModelTokens, CancellationToken cancellationToken)
    {
        var prompt = @$"Please translate from 'EN-us' to '{locale}' the following text: The length of the system prompt is {systemPromptTokens} and the length of the user ask is {askTokens}. The maximum length of the prompt and the user ask is {chatModelTokens}.";

        chatHistory.AddSystemMessage(prompt);

        var chatMessage = await kernel.GetRequiredService<IChatCompletionService>().GetChatMessageContentAsync(chatHistory, options.ChatRequestSettings, kernel, cancellationToken);

        return chatMessage.Content;
    }
}
