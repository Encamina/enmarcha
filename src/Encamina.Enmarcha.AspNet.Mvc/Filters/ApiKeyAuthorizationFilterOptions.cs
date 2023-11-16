namespace Encamina.Enmarcha.AspNet.Mvc.Filters;

/// <summary>
/// Configuration options for an API Key authorization filter.
/// </summary>
public class ApiKeyAuthorizationFilterOptions
{
    /// <summary>
    /// Gets or sets the expected name for the header containing the API Key value.
    /// </summary>
    public string HeaderApiKey { get; set; } = @"x-api-key";
}
