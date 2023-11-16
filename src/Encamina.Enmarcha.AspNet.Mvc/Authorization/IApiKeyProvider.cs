namespace Encamina.Enmarcha.AspNet.Mvc.Authorization;

/// <summary>
/// Represents an authorization provider for APIs that uses a pre-shared key (PSK) or API Key to grant or deny access to an specific API.
/// </summary>
public interface IApiKeyProvider
{
    /// <summary>
    /// Determines if the authorization is valid based on the API client unique identifier and the API key. If the given <paramref name="apiKey"/> is
    /// valid for the <paramref name="apiKeyClientId"/> then the authorization is granted, otherwise it will be denied.
    /// </summary>
    /// <param name="apiKeyClientId">The API key client unique identifier, which represents the identifier of the key used to authorize access to an API.</param>
    /// <param name="apiKey">The API key or pre-shared key (PSK) to validate.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the <paramref name="apiKey"/> is valid for the <paramref name="apiKeyClientId"/>, otherwise returns <see langword="false"/>.
    /// </returns>
    Task<bool> IsAuthorizedAsync(string apiKeyClientId, string apiKey, CancellationToken cancellationToken);
}