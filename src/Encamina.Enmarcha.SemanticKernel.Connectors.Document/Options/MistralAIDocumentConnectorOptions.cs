using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;
using Encamina.Enmarcha.SemanticKernel.Connectors.Document.Connectors;

namespace Encamina.Enmarcha.SemanticKernel.Connectors.Document.Options;

/// <summary>
/// Configuration options for <see cref="MistralAIDocumentConnector"/>.
/// </summary>
public sealed class MistralAIDocumentConnectorOptions
{
    /// <summary>
    /// Gets the endpoint URL for Mistral AI Document API.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets the API key for authentication.
    /// </summary>
    [Required]
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets the model name to use for document processing.
    /// </summary>
    [Required]
    public required string ModelName { get; init; }

    /// <summary>
    /// Gets the number of pages to split the PDF into for processing.
    /// </summary>
    [Range(1, 30)]
    public int SplitPageNumber { get; init; } = 30;

    /// <summary>
    /// Gets a value indicating whether to apply LLM-based post-processing to refine the extracted text.
    /// </summary>
    public bool LLMPostProcessing { get; init; } = true;
}
