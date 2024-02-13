namespace Encamina.Enmarcha.AspNet.OpenApi.Middlewares;

/// <summary>
/// Option parameters for the group name key authorization middleware.
/// </summary>
public sealed class GroupNameKeyAuthenticationOptions
{
    /// <summary>
    /// Gets or sets a dictionary of keys indexed by group name.
    /// </summary>
    public IDictionary<string, string> GroupNameKeys { get; set; }

    /// <summary>
    /// Gets or sets the specification file name. By default, it is set to <c>swagger.json</c> because it is the most common OpenAPI implementation.
    /// </summary>
    public string SpecificationFileName { get; set; } = @"swagger.json";

    /// <summary>
    /// Gets or sets the specification path. By default, it is set to <c>/swagger</c> because it is the most common OpenAPI implementation.
    /// </summary>
    /// <remarks>It is very important that the value of this property starts with '<c>/</c>'.</remarks>
    public string SpecificationPath { get; set; } = @"/swagger";

    /// <summary>
    /// Gets or sets a value indicating whether the middleware is enabled. By default, it is set to <c>true</c>.
    /// </summary>
    /// <remarks>This property would allow turning on/off the middleware for development or testing purposes.</remarks>
    public bool IsEnabled { get; set; } = true;
}
