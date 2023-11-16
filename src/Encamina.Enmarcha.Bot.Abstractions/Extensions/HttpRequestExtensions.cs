using Microsoft.AspNetCore.Http;

using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="HttpRequest"/>.
/// </summary>
public static class HttpRequestExtensions
{
    /// <summary>
    /// Obtains the <see cref="Activity"/> from the <see cref="HttpRequest"/> body.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> to obtain the <see cref="Activity"/> from.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task<Activity> GetActivityAsync(this HttpRequest request)
    {
        // Reset the request `Body` stream position so we can read it...
        request.Body.Position = 0;

        var activity = await HttpHelper.ReadRequestAsync<Activity>(request);

        // Reset the request `Body` stream position so the next operation can read it...
        request.Body.Position = 0;

        return activity;
    }
}
