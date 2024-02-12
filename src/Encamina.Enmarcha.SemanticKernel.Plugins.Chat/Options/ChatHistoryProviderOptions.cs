using System.ComponentModel.DataAnnotations;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;

/// <summary>
/// Configuration options for <see cref="ChatHistoryProvider"/>.
/// </summary>
public sealed class ChatHistoryProviderOptions
{
    /// <summary>
    /// Gets the maximum number of messages to load from the chat history.
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int HistoryMaxMessages { get; init; }
}
