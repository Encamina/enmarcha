#pragma warning disable IDE0060 // Remove unused parameter

using System.Text.Json;

namespace Encamina.Enmarcha.Core;

/// <summary>
/// Collection of static methods that provide utilities for JSON handling.
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// When using the <see cref="System.Text.Json"/> library, this method helps deserializing anonymous types from a JSON string.
    /// </summary>
    /// <remarks>
    /// The <paramref name="anonymousType"/> is required to identify the type of the anonymous type that is going to be deserialized.
    /// </remarks>
    /// <typeparam name="T">The anonymous type.</typeparam>
    /// <param name="json">The JSON string to deserialize as an instance of an anonymous type represented by <typeparamref name="T"/>.</param>
    /// <param name="anonymousType">The anonymous type that identifies the type to deserialize from a JSON string.</param>
    /// <param name="options">A valid instance of <see cref="JsonSerializerOptions"/> with options for the deserialization.</param>
    /// <returns>A valid instance of <typeparamref name="T"/> obtained from the JSON deserialization.</returns>
    public static T? DeserializeAnonymousType<T>(string json, T anonymousType, JsonSerializerOptions? options = default) => JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Performs a fast check to determine if the input string is a JSON object.
    /// This method only checks if the string starts with '{' and ends with '}'.
    /// It does not validate the entire JSON structure.
    /// </summary>
    /// <param name="input">The input string to check.</param>
    /// <returns>
    /// <c>true</c> if the input starts with '{' and ends with '}'; otherwise, <c>false</c>.
    /// </returns>
    public static bool FastCheckIsJson(string input)
    {
        input = input.Trim();
        return input.StartsWith('{') && input.EndsWith('}');
    }

    /// <summary>
    /// Determines whether the given JSON string represents an Adaptive Card.
    /// An Adaptive Card must have a 'type' property with the value 'AdaptiveCard' and a Adaptive Card schema '$schema'.
    /// </summary>
    /// <param name="input">The input string to check, which is expected to be a JSON object.</param>
    /// <returns>
    /// <c>true</c> if the input is a valid JSON object representing an Adaptive Card; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAnAdaptiveCard(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if (!FastCheckIsJson(input))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(input);
            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            // Check for required properties: 'type' and '$schema'
            if (!root.TryGetProperty("type", out var typeElement) ||
                !root.TryGetProperty("$schema", out var schemaElement))
            {
                return false;
            }

            return string.Equals(typeElement.GetString(), @"AdaptiveCard", StringComparison.OrdinalIgnoreCase) &&
                   schemaElement.GetString()!.EndsWith(@"adaptivecards.io/schemas/adaptive-card.json", StringComparison.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            // input is not a valid JSON object
            return false;
        }
        catch (InvalidOperationException)
        {
            // Handle potential exceptions due to invalid JSON structure
            return false;
        }
    }
}

#pragma warning restore IDE0060
