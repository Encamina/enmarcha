using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AspNet.Mvc.Authorization;

/// <summary>
/// A basic API Key provider.
/// </summary>
internal sealed class BasicApiKeyProvider : IApiKeyProvider
{
    private readonly IDictionary<string, string> values;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicApiKeyProvider"/> class.
    /// </summary>
    /// <param name="options">Configuration options for this API Key provider.</param>
    public BasicApiKeyProvider(IOptions<BasicApiKeyOptions> options)
    {
        values = options.Value.ApiKeys;
    }

    /// <inheritdoc/>
    public Task<bool> IsAuthorizedAsync(string apiKeyClientId, string apiKey, CancellationToken cancellationToken)
    {
        return Task.FromResult(values != null &&
                               !string.IsNullOrWhiteSpace(apiKeyClientId) &&
                               values.TryGetValue(apiKeyClientId, out var value) &&
                               apiKey.Equals(value, StringComparison.OrdinalIgnoreCase));
    }
}