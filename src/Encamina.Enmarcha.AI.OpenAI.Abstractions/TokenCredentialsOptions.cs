using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Core.DataAnnotations;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Represents the options for token credentials used in authentication.
/// </summary>
public class TokenCredentialsOptions
{
    /// <summary>
    /// Gets or sets the tenant ID.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    [Required]
    [NotEmptyOrWhitespace]
    public string ClientSecret { get; set; }
}
