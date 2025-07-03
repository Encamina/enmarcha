namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// <para>
/// Represents a collection of functions to count or calculate the length (or size) of a text.
/// <b>This interface is not expected to be implemented directly, but rather used as a «mixin».</b>
/// </para>
/// <para>
/// References:
/// </para>
/// <list type = "bullet">
///     <item>
///         <description><see href="https://en.wikipedia.org/wiki/Mixin"/></description>
///     </item>
///     <item>
///         <description><see href="https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interface-implementation/mixins-with-default-interface-methods"/></description>
///     </item>
/// </list>
/// </summary>
public interface ILengthFunctions
{
    /// <summary>
    /// Gets the number of characters in the specified text. If the text is <see langword="null"/>, returns zero (<c>0</c>).
    /// </summary>
    public static Func<string, int> LengthByCharacterCount => (text) => text?.Length ?? 0;
}
