using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Base class with options to connect and use an OpenAI service.
/// </summary>
public class OpenAIOptionsBase
{
    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for chat.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: The model deployment name does not necessarily have to be the same as the model name. For example, a model of type `gpt-4` might be called «MyGPT»;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="ChatModelName"/> to set the model name.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string? ChatModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for chat.
    /// </summary>
    /// <remarks>
    /// This property is required if property <see cref="ChatModelDeploymentName"/>  is not <see langword="null"/>. It is usually used with the <c>Encamina.Enmarcha.AI.OpenAI.Abstractions.ModelInfo"</c> class
    /// to get metadata and information about the model. This model name must match the model names from the LLM (like OpenAI), like for example `gpt-4` or `gpt-35-turbo`.
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(ChatModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string? ChatModelName { get; init; }

    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for completions.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: The model name does not necessarily have to be the same as the model ID. For example, a model of type `text-davinci-003` might be called `MyCompletions`;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="CompletionsModelName"/> to set the model name.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string? CompletionsModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for completions.
    /// </summary>
    /// <remarks>
    /// This property is required if property <see cref="CompletionsModelDeploymentName"/> is not <see langword="null"/>. It is usually used with the <c>Encamina.Enmarcha.AI.OpenAI.Abstractions.ModelInfo"</c> class
    /// to get metadata and information about the model. This model name must match the model names from the LLM (like OpenAI), like for example `gpt-4` or `gpt-35-turbo`.
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(CompletionsModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string? CompletionsModelName { get; init; }

    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for embeddings.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: The model name does not necessarily have to be the same as the model ID. For example, a model of type `text-embedding-ada-002` might be called `MyEmbeddings`;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="EmbeddingsModelName"/> to set the model name.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string? EmbeddingsModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for embeddings.
    /// </summary>
    /// <remarks>
    /// This property is required if property <see cref="EmbeddingsModelDeploymentName"/> is not <see langword="null"/>. It is usually used with the <c>Encamina.Enmarcha.AI.OpenAI.Abstractions.ModelInfo"</c> class
    /// to get metadata and information about the model. This model name must match the model names from the LLM (like OpenAI), like for example `gpt-4` or `gpt-35-turbo`.
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(EmbeddingsModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string? EmbeddingsModelName { get; init; }
}
