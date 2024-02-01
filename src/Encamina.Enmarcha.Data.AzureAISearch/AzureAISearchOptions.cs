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
    [Required]
    public required string Key { get; init; }
}