using System.Text.Json;
using System.Text.Json.Serialization;

namespace Encamina.Enmarcha.Core;

/// <summary>
/// Provides functionality to convert between JSON values and <see langword="bool"/> values during serialization and
/// deserialization.
/// </summary>
/// <remarks>This converter supports deserializing JSON boolean values (<see langword="true"/> or <see
/// langword="false"/>)  as well as string representations of boolean values ("True" or "False", case-insensitive).  If
/// an invalid string value is encountered during deserialization, a <see cref="JsonException"/> is thrown. During
/// serialization, boolean values are written as JSON boolean literals.</remarks>
public class StringToBooleanConverter : JsonConverter<bool>
{
    /// <summary>
    /// Reads and converts a JSON value to a boolean.
    /// </summary>
    /// <inheritdoc/>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;

            case JsonTokenType.False:
                return false;

            case JsonTokenType.String:
                {
                    var value = reader.GetString();

                    return string.Equals(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) || (string.Equals(value, bool.FalseString, StringComparison.OrdinalIgnoreCase)
                        ? false
                        : throw new JsonException($"Invalid string value for boolean: {value}"));
                }

            default:
                throw new JsonException($"Invalid token type: {reader.TokenType}");
        }
    }

    /// <summary>
    /// Writes a boolean value to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}
