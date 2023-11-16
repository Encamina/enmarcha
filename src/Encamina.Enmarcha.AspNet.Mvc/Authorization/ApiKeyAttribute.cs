using Microsoft.AspNetCore.Mvc;

namespace Encamina.Enmarcha.AspNet.Mvc.Authorization;

/// <summary>
/// Decorates a class (usually a <see cref="Controller"/>) or mehtod (usually a <see cref="Controller"/>'s action) with an API key client unique identifier
/// that points to the key required to authiruze access to the API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ApiKeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAttribute"/> class.
    /// </summary>
    /// <param name="apiKeyName">The API key name, usually represented as an API client unique identifier, which represents the name of the key used to authorize access to an API.</param>
    public ApiKeyAttribute(string apiKeyName)
    {
        ApiKeyName = apiKeyName;
    }

    /// <summary>
    /// Gets the API key name.
    /// </summary>
    public string ApiKeyName { get; init; }
}
