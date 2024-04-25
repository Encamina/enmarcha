using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Options for configuring access to OpenAI services.
/// </summary>
public class OpenAIOptions : OpenAIOptionsBase
{
    /// <summary>
    /// Gets the key credential used to authenticate to an LLM resource.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string Key { get; init; }
}
