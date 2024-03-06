using System.ComponentModel.DataAnnotations;

using Azure.AI.OpenAI;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.AI.OpenAI.Azure;

/// <summary>
/// Configuration options for Azure OpenAI service connection.
/// </summary>
public sealed class AzureOpenAIOptions : OpenAIOptions
{
    /// <summary>
    /// Gets the Azure OpenAI API service version.
    /// </summary>
    public OpenAIClientOptions.ServiceVersion ServiceVersion { get; init; } = OpenAIClientOptions.ServiceVersion.V2023_05_15;

    /// <summary>
    /// Gets the <see cref="System.Uri "/> for an LLM resource (like OpenAI). This should include protocol and host name.
    /// </summary>
    [Required]
    [Uri]
    public Uri Endpoint { get; init; }
}

