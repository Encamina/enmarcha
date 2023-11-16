using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base record for cognitive service option records.
/// </summary>
public record CognitiveServiceOptionsBase : INameable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CognitiveServiceOptionsBase"/> class.
    /// </summary>
    protected CognitiveServiceOptionsBase()
    {
    }

    /// <summary>
    /// Gets the name of this cognitive service.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Name { get; init; }

    /// <summary>
    /// Gets the endpoint URL for this cognitive service.
    /// </summary>
    [Required]
    [Url]
    public Uri EndpointUrl { get; init; }

    /// <summary>
    /// Gets the key credential to use to authenticate with the Azure service.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string KeyCredential { get; init; }
}
