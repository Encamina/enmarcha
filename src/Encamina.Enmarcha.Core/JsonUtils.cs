using System.Text.Json;

namespace Encamina.Enmarcha.Core;

/// <summary>
/// Collection of static methods that provide utilities for JSON handling.
/// </summary>
public static class JsonUtils
{
#pragma warning disable IDE0060 // Remove unused parameter

    /// <summary>
    /// When using the <see cref="System.Text.Json"/> library, this method helps deserializing anonymous types from a JSON string.
    /// </summary>
    /// <remarks>
    /// The <paramref name="anonymousType"/> is required to identify the type of the anonymous type that is going to be deserialized.
    /// </remarks>
    /// <typeparam name="T">The anonyumous type.</typeparam>
    /// <param name="json">The JSON string to deserialize as an instance of an anonymous type represented by <typeparamref name="T"/>.</param>
    /// <param name="anonymousType">The anonymous type that identifies the type to deserialize from a JSON string.</param>
    /// <param name="options">A valid instance of <see cref="JsonSerializerOptions"/> with options for the deserialization.</param>
    /// <returns>A valid instance of <typeparamref name="T"/> obtained from the JSON deserialization.</returns>
    public static T DeserializeAnonymousType<T>(string json, T anonymousType, JsonSerializerOptions options = default) => JsonSerializer.Deserialize<T>(json, options);

#pragma warning restore IDE0060
}
