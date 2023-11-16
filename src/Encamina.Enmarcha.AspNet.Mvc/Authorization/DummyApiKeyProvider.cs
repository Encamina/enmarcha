namespace Encamina.Enmarcha.AspNet.Mvc.Authorization;

internal sealed class DummyApiKeyProvider : IApiKeyProvider
{
    public DummyApiKeyProvider()
    {
    }

    /// <inheritdoc/>
    public Task<bool> IsAuthorizedAsync(string apiKeyClientId, string apiKey, CancellationToken cancellationToken)
    {
        return Task.FromResult(!(string.IsNullOrWhiteSpace(apiKeyClientId) || string.IsNullOrWhiteSpace(apiKey)));
    }
}

