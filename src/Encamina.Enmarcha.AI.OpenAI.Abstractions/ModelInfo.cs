using System.Collections.ObjectModel;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Provides information about a model.
/// </summary>
/// <seealso href="https://platform.openai.com/docs/models/overview"/>
/// <seealso href="https://learn.microsoft.com/en-us/azure/cognitive-services/openai/concepts/models"/>
/// <seealso href="https://techcommunity.microsoft.com/t5/azure-ai-services-blog/announcing-updates-to-azure-openai-service-models/ba-p/3866757"/>
public sealed class ModelInfo
{
    private static readonly IDictionary<string, ModelInfo> ModelInfoById = new ReadOnlyDictionary<string, ModelInfo>(new Dictionary<string, ModelInfo>()
    {
        // Chat...
        { @"gpt-35-turbo-16k", new ModelInfo() { Id = @"gpt-35-turbo-16k", MaxTokens = 16200, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-3.5-turbo-16k", new ModelInfo() { Id = @"gpt-3.5-turbo-16k", MaxTokens = 16200, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-35-turbo", new ModelInfo() { Id = @"gpt-35-turbo", MaxTokens = 4096, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-3.5-turbo", new ModelInfo() { Id = @"gpt-3.5-turbo", MaxTokens = 4096, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-4", new ModelInfo() { Id = @"gpt-4", MaxTokens = 8192, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-4-32k", new ModelInfo() { Id = @"gpt-4", MaxTokens = 32768, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-4-turbo", new ModelInfo() { Id = @"gpt-4", MaxTokens = 128000, Encoding = @"cl100k_base", IsObsolete = false } },
        { @"gpt-4o", new ModelInfo() { Id = @"gpt-4o", MaxTokens = 128000, Encoding = @"o200k_base", IsObsolete = false } },
        { @"text-davinci-001", new ModelInfo() { Id = @"text-davinci-001", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-davinci-002", new ModelInfo() { Id = @"text-davinci-002", MaxTokens = 4097, Encoding = @"p50k_base", IsObsolete = true } },
        { @"text-davinci-003", new ModelInfo() { Id = @"text-davinci-003", MaxTokens = 4097, Encoding = @"p50k_base", IsObsolete = true } },
        { @"text-curie-001", new ModelInfo() { Id = @"text-curie-001", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-babbage-001", new ModelInfo() { Id = @"text-babbage-001", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-ada-001", new ModelInfo() { Id = @"text-ada-001", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"davinci", new ModelInfo() { Id = @"davinci", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"curie", new ModelInfo() { Id = @"curie", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"babbage", new ModelInfo() { Id = @"babbage", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },
        { @"ada", new ModelInfo() { Id = @"ada", MaxTokens = 2049, Encoding = @"r50k_base", IsObsolete = true } },

        // Code...
        { @"code-davinci-001", new ModelInfo() { Id = @"code-davinci-001", MaxTokens = 8001, Encoding = @"p50k_base", IsObsolete = true } },
        { @"code-davinci-002", new ModelInfo() { Id = @"code-davinci-002", MaxTokens = 8001, Encoding = @"p50k_base", IsObsolete = true } },
        { @"code-cushman-001", new ModelInfo() { Id = @"code-cushman-001", MaxTokens = 2048, Encoding = @"p50k_base", IsObsolete = true } },
        { @"code-cushman-002", new ModelInfo() { Id = @"code-cushman-002", MaxTokens = 2048, Encoding = @"p50k_base", IsObsolete = true } },

        // Embeddings...
        { @"text-embedding-ada-002", new ModelInfo() { Id = @"text-embedding-ada-002", MaxTokens = 8191, Encoding = @"cl100k_base", IsObsolete = false } },

        // Legacy...
        { @"text-similarity-davinci-001", new ModelInfo() { Id = @"text-similarity-davinci-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-similarity-curie-001", new ModelInfo() { Id = @"text-similarity-curie-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-similarity-babbage-001", new ModelInfo() { Id = @"text-similarity-babbage-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-similarity-ada-001", new ModelInfo() { Id = @"text-similarity-ada-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-search-davinci-doc-001", new ModelInfo() { Id = @"text-search-davinci-doc-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-search-curie-doc-001", new ModelInfo() { Id = @"text-search-curie-doc-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-search-babbage-doc-001", new ModelInfo() { Id = @"text-search-babbage-doc-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"text-search-ada-doc-001", new ModelInfo() { Id = @"text-search-ada-doc-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"code-search-babbage-code-001", new ModelInfo() { Id = @"code-search-babbage-code-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
        { @"code-search-ada-code-001", new ModelInfo() { Id = @"code-search-ada-code-001", MaxTokens = 2046, Encoding = @"r50k_base", IsObsolete = true } },
    });

    /// <summary>
    /// Gets the unique identifier of the model.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets the maximum number of tokens allowed for the model.
    /// </summary>
    public int MaxTokens { get; init; }

    /// <summary>
    /// Gets the supported encoding for the model.
    /// </summary>
    public string Encoding { get; init; }

    /// <summary>
    /// Gets a value indicating whether the model is obsolete or not.
    /// </summary>
    /// <remarks>
    /// An obsolete model does not mean that it is not available, just that it should no longer be used because it has been or is planned to be retired.
    /// </remarks>
    public bool IsObsolete { get; init; }

    /// <summary>
    /// Gets the model information by its unique identifier, like for example '<c>text-embedding-ada-002</c>' or '<c>gpt-4</c>'.
    /// </summary>
    /// <param name="id">The model's unique identifier. For example <c>text-embedding-ada-002</c> or <c>gpt-4</c>.</param>
    /// <returns>A models information from the given unique identifier, or <see langword="null"/> if it is not found.</returns>
    public static ModelInfo? GetById(string id) => ModelInfoById.TryGetValue(id, out var modelInfo) ? modelInfo : null;
}
