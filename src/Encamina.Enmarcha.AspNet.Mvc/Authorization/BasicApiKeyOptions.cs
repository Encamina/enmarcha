namespace Encamina.Enmarcha.AspNet.Mvc.Authorization;

/// <summary>
/// Configuration options for the <see cref="BasicApiKeyProvider"/>.
/// </summary>
public class BasicApiKeyOptions
{
    /// <summary>
    /// Gets or sets the dictionary that relates an API key client unique identifier its expected API key.
    /// </summary>
    public IDictionary<string, string> ApiKeys { get; set; }
}
