using System.ComponentModel.DataAnnotations;

using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

/// <summary>
/// Configuration options for the <see cref="ChatWithHistoryPlugin"/>.
/// </summary>
public class ChatWithHistoryPluginOptions
{
    /// <summary>
    /// Gets the maximum number of messages to load from the chat history.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public virtual int HistoryMaxMessages { get; init; }

    /// <summary>
    /// Gets a valid instance of <see cref="OpenAIRequestSettings"/> (from Semantic Kernel) with settings for the chat request.
    /// </summary>
    [Required]
    public OpenAIRequestSettings ChatRequestSettings { get; init; } = new()
    {
        MaxTokens = 1000,
        Temperature = 0.8,
        TopP = 0.5,
    };
}
