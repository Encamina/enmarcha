using System.ComponentModel.DataAnnotations;

using Azure.AI.OpenAI;

using Encamina.Enmarcha.Core.DataAnnotations;
using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.OpenAI.Azure;

/// <summary>
/// Options to configure an OpenAI Completion service for Azure.
/// </summary>
public class CompletionServiceOptions : OpenAIClientOptions, IIdentifiable<string>, INameable
{
    /// <summary>
    /// Gets or sets the name of the deployed model to use with this completion service.
    /// </summary>
    /// <remarks>
    /// This will correspond to the custom name choosen for a deployment when deploying a model.
    /// This value can be found under «Resource Management → Deployments» in the Azure portal, or alternatively under «Management > Deployments» in Azure OpenAI Studio.
    /// </remarks>
    [Required(AllowEmptyStrings = false)]
    public string DeploymentName { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URL for this completion service.
    /// </summary>
    /// <remarks>
    /// For Azure, this value can be found in the «Keys &amp; Endpoint» section when examining the OpenAI resource from the Azure portal.
    /// Alternatively, it can be found under «Playground → Code View» in Azure OpenAI Studio.
    /// </remarks>
    [Required]
    [Uri]
    public Uri EndpointUrl { get; set; }

    /// <summary>
    /// Gets or sets the name of this completion service.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; set; }

    /// <inheritdoc/>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the key credential, usually an API key, to authenticate with the Azure service.
    /// </summary>
    /// <remarks>
    /// This value can be found in the «Keys &amp; Endpoint» section when examining the OpenAI resource from the Azure portal.
    /// </remarks>
    [Required(AllowEmptyStrings = false)]
    public string KeyCredential { get; set; }

    /// <inheritdoc/>
    object IIdentifiable.Id => Id;
}
