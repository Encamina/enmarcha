namespace Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle;

/// <summary>
/// Configuration options for the version support OpenAPI.
/// </summary>
public class VersionSwaggerGenOptions
{
    /// <summary>
    /// Gets or sets the title for the OpenAPI document when using version support.
    /// </summary>
    public required string Title { get; set; } = @"REST API";

    /// <summary>
    /// Gets or sets the description for the OpenAPI document when using version support.
    /// </summary>
    public string Description { get; set; }
}