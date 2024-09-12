using System.ComponentModel.DataAnnotations;

using Azure.AI.OpenAI;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;

using Encamina.Enmarcha.Core;
using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.AI.OpenAI.Azure;

/// <summary>
/// Configuration options for Azure OpenAI service connection.
/// </summary>
public sealed class AzureOpenAIOptions : OpenAIOptionsBase
{
    /// <summary>
    /// Gets the Azure OpenAI API service version.
    /// </summary>
    public OpenAIClientOptions.ServiceVersion ServiceVersion { get; init; } = OpenAIClientOptions.ServiceVersion.V2024_02_15_Preview;

    /// <summary>
    /// Gets the <see cref="Uri "/> for an LLM resource (like OpenAI). This should include protocol and host name.
    /// </summary>
    [Required]
    [Uri]
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the key credential used to authenticate to an LLM resource.
    /// This property is required if property <see cref="UseTokenCredentialAuthentication"/> is <c>false</c>.
    /// </summary>
    [RequiredIf([nameof(UseTokenCredentialAuthentication), nameof(UseDefaultAzureCredentialAuthentication)], [false, false], failOnAnyCondition: false)]
    public string? Key { get; init; }

    /// <summary>
    /// Gets a value indicating whether Token Credential authentication should be used.
    /// If set to <see langword="true"/>, the value of the <see cref="Key"/> property is ignored.
    /// </summary>
    public bool UseTokenCredentialAuthentication { get; init; }

    /// <summary>
    /// Gets a value indicating whether Default Azure Credential authentication should be used.
    /// If set to <see langword="true"/>, the value of the <see cref="Key"/> property is ignored.
    /// </summary>
    public bool UseDefaultAzureCredentialAuthentication { get; init; } = false;

    /// <summary>
    /// Gets the token credentials options to authenticate to an LLM resource.
    /// </summary>
    public TokenCredentialsOptions? TokenCredentialsOptions { get; init; }
}
