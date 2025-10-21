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
    /// Gets or sets the endpoint URL for Mistral AI Document API.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    [Required]
    public string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the model name to use for document processing.
    /// </summary>
    [Required]
    public string ModelName { get; set; }

    /// <summary>
    /// Gets or sets the number of pages to split the PDF into for processing.
    /// </summary>
    [Range(1, 30)]
    public int SplitPageNumber { get; set; } = 30;

    /// <summary>
    /// Gets or sets a value indicating whether to apply LLM-based post-processing to refine the extracted text.
    /// </summary>
    public bool LLMPostProcessing { get; set; } = true;
}
