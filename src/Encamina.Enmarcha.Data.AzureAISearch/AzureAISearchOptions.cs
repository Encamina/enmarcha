using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.Data.AzureAISearch;

/// <summary>
/// Configuration options for setting up a connection to an Azure AI Search, and use it as a vector database.
/// </summary>
public sealed class AzureAISearchOptions
{
    /// <summary>
    /// Gets the Azure AI Search endpoint, , e.g. "https://enmarcha.search.windows.net".
    /// </summary>
    [Required]
    [Uri]
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the Azure AI Search key.
    /// </summary>
    [RequiredIf(nameof(UseTokenCredentialAuthentication), false)]
    public string? Key { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether token credential authentication should be used.
    /// When set to <see langword="true"/>, a TokenCredential must be provided to the Azure AI Search client constructor at run-time.
    /// </summary>
    /// <remarks>
    /// Default value is <see langword="false" />.
    /// </remarks>
    public bool UseTokenCredentialAuthentication { get; set; } = false;
}