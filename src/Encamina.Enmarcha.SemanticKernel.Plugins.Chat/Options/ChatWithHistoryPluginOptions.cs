using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;

/// <summary>
/// Configuration options for the <see cref="ChatWithHistoryPlugin"/>.
/// </summary>
public class ChatWithHistoryPluginOptions
{
    /// <summary>
    /// Gets a valid instance of <see cref="OpenAIPromptExecutionSettings"/> (from Semantic Kernel) with settings for the chat request.
    /// </summary>
    [Required]
    public virtual OpenAIPromptExecutionSettings ChatRequestSettings { get; init; } = new()
    {
        MaxTokens = 1000,
        Temperature = 0.8,
        TopP = 0.5,
    };
}
