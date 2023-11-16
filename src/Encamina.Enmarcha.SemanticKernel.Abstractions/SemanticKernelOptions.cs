using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Options to configure a Semantic Kernel.
/// </summary>
/// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/overview/"/>
public sealed class SemanticKernelOptions
{
    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for chat.
    /// </summary>
    /// <remarks>
    /// <b>WARNING:</b> The model deployment name does not necessarily have to be the same as the model name. For example, a model of type «GPT-4» might be called «MyGPT»;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it. Use property <see cref="ChatModelName"/> to set the
    /// model name.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string ChatModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for chat.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is required if <see cref="ChatModelDeploymentName"/> is not <see langword="null"/>.
    /// </para>
    /// <para>
    /// This model name must match the model names from the LLM (like OpenAI), like for example <c>gpt-4</c> or <c>gpt-35-turbo</c>.
    /// </para>
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(ChatModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string ChatModelName { get; init; }

    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for completions.
    /// </summary>
    /// <remarks>
    /// <b>WARNING:</b> The model name does not necessarily have to be the same as the model ID. For example, a model of type «text-davinci-003» might be called «MyCompletions»;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string CompletionsModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for completions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is required if <see cref="CompletionsModelDeploymentName"/> is not <see langword="null"/>.
    /// </para>
    /// <para>
    /// This model name must match the model names from the LLM (like OpenAI), like for example <c>gpt-4</c> or <c>gpt-35-turbo</c>.
    /// </para>
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(CompletionsModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string CompletionsModelName { get; init; }

    /// <summary>
    /// Gets the model deployment name on the LLM (for example OpenAI) to use for embddings.
    /// </summary>
    /// <remarks>
    /// <b>WARNING:</b> The model name does not necessarily have to be the same as the model ID. For example, a model of type «text-embedding-ada-002» might be called «MyEmbeddings»;
    /// this means that the value of this property does not necessarily indicate the model implemented behind it.
    /// </remarks>
    [NotEmptyOrWhitespace]
    public string EmbeddingsModelDeploymentName { get; init; }

    /// <summary>
    /// Gets the name (sort of a unique identifier) of the model to use for embeddings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is required if <see cref="EmbeddingsModelDeploymentName"/> is not <see langword="null"/>.
    /// </para>
    /// <para>
    /// This model name must match the model names from the LLM (like OpenAI), like for example <c>gpt-4</c> or <c>gpt-35-turbo</c>.
    /// </para>
    /// </remarks>
    [RequireWhenOtherPropertyNotNull(nameof(EmbeddingsModelDeploymentName))]
    [NotEmptyOrWhitespace]
    public string EmbeddingsModelName { get; init; }

    /// <summary>
    /// Gets the <see cref="Uri"/> for an LLM resource (like OpenAI). This should include protocol and hostname.
    /// </summary>
    [Required]
    [Uri]
    public Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the key credential used to authenticate to an LLM resource.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Key { get; init; }
}
