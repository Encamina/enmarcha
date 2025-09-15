using Microsoft.Agents.Authentication;

namespace Encamina.Enmarcha.Agents.Options;

/// <summary>
/// Options for incoming token validation.
/// </summary>
public class TokenValidationOptions
{
    /// <summary>
    /// Gets the list of valid audiences.
    /// </summary>
    public IList<string>? Audiences { get; init; }

    /// <summary>
    /// Gets the TenantId of the Azure Bot. Optional but recommended.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Gets or sets the additional valid issuers. Optional, in which case the Public Azure Bot Service issuers are used.
    /// </summary>
    public IList<string>? ValidIssuers { get; set; }

    /// <summary>
    /// Gets a value indicating whether it can be omitted, in which case public Azure Bot Service and Azure Cloud metadata urls are used.
    /// </summary>
    public bool IsGov { get; init; }

    /// <summary>
    /// Gets or sets Azure Bot Service OpenIdMetadataUrl. Optional, in which case default value depends on IsGov.
    /// </summary>
    /// <see cref="AuthenticationConstants.PublicAzureBotServiceOpenIdMetadataUrl"/>
    /// <see cref="AuthenticationConstants.GovAzureBotServiceOpenIdMetadataUrl"/>
    public string? AzureBotServiceOpenIdMetadataUrl { get; set; }

    /// <summary>
    /// Gets or sets Entra OpenIdMetadataUrl. Optional, in which case default value depends on IsGov.
    /// </summary>
    /// <see cref="AuthenticationConstants.PublicOpenIdMetadataUrl"/>
    /// <see cref="AuthenticationConstants.GovOpenIdMetadataUrl"/>
    public string? OpenIdMetadataUrl { get; set; }

    /// <summary>
    /// Gets a value indicating whether Azure Bot Service tokens are handled. Defaults to true and should always be true until Azure Bot Service sends Entra ID token.
    /// </summary>
    public bool AzureBotServiceTokenHandling { get; init; } = true;

    /// <summary>
    /// Gets OpenIdMetadata refresh interval. Defaults to 12 hours.
    /// </summary>
    public TimeSpan? OpenIdMetadataRefresh { get; init; }
}