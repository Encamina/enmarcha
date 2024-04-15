using System.Text.Json.Serialization;

using Encamina.Enmarcha.Entities.Abstractions;

using Newtonsoft.Json;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

/// <summary>
/// Represents an chat message historical record.
/// </summary>
public class ChatMessageHistoryRecord : IIdentifiable<string>
{
    /// <inheritdoc/>
    [JsonProperty(@"id")]
    [JsonPropertyName(@"id")]
    public virtual string Id { get; init; }

    /// <summary>
    /// Gets the unique identifier of the chat history indexer.
    /// </summary>
    /// <remarks>
    /// Identifier that relates several messages. This could be, for example, a conversationId, a userId, or any other relevant identifier.
    /// </remarks>
    [JsonProperty(@"indexerId")]
    [JsonPropertyName(@"indexerId")]
    public virtual string IndexerId { get; init; }

    /// <summary>
    /// Gets the name of the role associated with the chat message.
    /// For example the `user`, the `assistant` or the `system`.
    /// </summary>
    [JsonProperty(@"roleName")]
    [JsonPropertyName(@"roleName")]
    public virtual string RoleName { get; init; }

    /// <summary>
    /// Gets the chat message.
    /// </summary>
    [JsonProperty(@"message")]
    [JsonPropertyName(@"message")]
    public virtual string Message { get; init; }

    /// <summary>
    /// Gets the timestamp of the message, in UTC.
    /// </summary>
    [JsonProperty(@"timestampUTC")]
    [JsonPropertyName(@"timestampUTC")]
    public virtual DateTime TimestampUtc { get; init; }

    /// <inheritdoc/>
    object IIdentifiable.Id => Id;
}
