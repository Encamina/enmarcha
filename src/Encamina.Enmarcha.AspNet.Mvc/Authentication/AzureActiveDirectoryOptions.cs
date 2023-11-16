namespace Encamina.Enmarcha.AspNet.Mvc.Authentication;

/// <summary>
/// Options required to configure Azure Active Directory authentication.
/// </summary>
public class AzureActiveDirectoryOptions
{
    /// <summary>
    /// Gets or sets the Azure Active Directory client's ID (sometimes also called Application ID).
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client's secret on Azure Active Diretory.
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the instance.
    /// </summary>
    public string Instance { get; set; }

    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Gets or sets the Azure's tenant ID.
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// Gets or sets a callback path, which sometimes is just an URL.
    /// </summary>
    public string CallbackPath { get; set; }
}
