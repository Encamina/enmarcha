using System.ComponentModel.DataAnnotations;

using Swashbuckle.AspNetCore.Annotations;

namespace Encamina.Enmarcha.Agents.Models;

/// <summary>
/// Common request for all AI Agents.
/// </summary>
public class AgentRequest
{
    /// <summary>
    /// Gets the user inquiry.
    /// </summary>
    [Required]
    [SwaggerParameter("The user inquiry, input or request.")]
    public virtual string Input { get; init; } = string.Empty;

    /// <summary>
    /// Gets the locale of the user.
    /// </summary>
    [Required]
    [SwaggerParameter("The language of the user.")]
    public virtual string Locale { get; init; } = string.Empty;
}
