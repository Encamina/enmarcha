using System.Text.Json;

using CommunityToolkit.Diagnostics;

using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Core.Serialization;
using Microsoft.AspNetCore.Http;

namespace Encamina.Enmarcha.Agents.Abstractions.Extensions;

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

        var activity = await ReadRequestAsync<Activity>(request);

        // Reset the request `Body` stream position so the next operation can read it...
        request.Body.Position = 0;

        return activity;
    }

    /// <summary>
    /// Accepts an incoming HttpRequest and deserializes it using the <see cref="ProtocolJsonSerializer"/>.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the request into.</typeparam>
    /// <param name="request">The HttpRequest.</param>
    /// <returns>The deserialized request.</returns>
    private static async Task<T> ReadRequestAsync<T>(HttpRequest request)
    {
        try
        {
            Guard.IsNotNull(request);

            using var memoryStream = new MemoryStream();
            await request.Body.CopyToAsync(memoryStream).ConfigureAwait(false);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return ProtocolJsonSerializer.ToObject<T>(memoryStream);
        }
        catch (JsonException)
        {
            return default!;
        }
    }
}
