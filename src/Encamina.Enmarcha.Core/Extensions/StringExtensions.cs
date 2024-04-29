using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for handling strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Normalizes a given string, removing diacritics, plus some unwanted charaters, and replacing others.
    /// </summary>
    /// <param name="value">The <see cref="string"/> value to normalize.</param>
    /// <param name="removeCharacters">A collection of characters to remove from <paramref name="value"/>.</param>
    /// <param name="replaceCharacters">A collection of characters to replace while normalizing <paramref name="value"/>.</param>
    /// <returns>The normalized string value.</returns>
    /// <example>
    /// <![CDATA[var normalized = "¿cámp!äñà?".Normalize(new List<char>() { '?', '¿', '!', '¡' }, new Dictionary<char, char>() { { (char)771, 'ñ' } }); // Returns 'campaña']]>
    /// </example>
    public static string NormalizeDiacritics(this string value, IEnumerable<char> removeCharacters, IDictionary<char, char> replaceCharacters)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length);
        var normalized = value.RemoveCharacters(removeCharacters).Normalize(NormalizationForm.FormD);

        foreach (var character in normalized)
        {
            if (replaceCharacters?.TryGetValue(character, out var replaceCharacter) ?? false)
            {
                builder.Length--;
                builder.Append(replaceCharacter);
            }
            else if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
            else
            {
                /* Previous conditions are required to be mutually exclusive */
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).Trim();
    }

    /// <summary>
    /// Removes the <c>Async</c> suffix from a string, if present.
    /// </summary>
    /// <remarks>
    /// This extension method is usefull when manipulating a an asynchronous method or function name.
    /// </remarks>
    /// <param name="value">The string value to remove the <c>Async</c> suffix from.</param>
    /// <returns>The value with the <c>Async</c> suffix removed, if it was present.</returns>
    public static string RemoveAsyncSuffix(this string value)
    {
        const string AsyncSuffix = @"Async";

        var lengthAsyncSuffix = AsyncSuffix.Length;

        if (value.EndsWith(AsyncSuffix, StringComparison.Ordinal) && value.Length > lengthAsyncSuffix)
        {
            value = value[..^lengthAsyncSuffix];
        }

        return value;
    }

    /// <summary>
    /// Removes some given characters from a string.
    /// </summary>
    /// <param name="value">The <see cref="string"/> value to remove characters from.</param>
    /// <param name="removeCharacters">A collection of characters to remove from <paramref name="value"/>.</param>
    /// <returns>The string value with characters removed.</returns>
    public static string RemoveCharacters(this string value, IEnumerable<char> removeCharacters)
    {
        return removeCharacters?.Any() ?? true ? string.Join(string.Empty, value.ToCharArray().Where(c => !removeCharacters.Contains(c))) : value;
    }

    /// <summary>
    /// Trims a string, and returns <see langword="null"/> if trimming results in an empty string.
    /// </summary>
    /// <param name="value">The string value to trim.</param>
    /// <returns>
    /// Returns the trimmed string, or <see langword="null"/> if the trimming result is an empty string, or if <paramref name="value"/> is <see langword="null"/> or empty as well.
    /// </returns>
    public static string? TrimAndAsNullIfEmpty(this string value)
    {
        var result = string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        return string.IsNullOrEmpty(result) ? null : result;
    }

    /// <summary>
    /// Formats a string as template with tokens or parameters provided from the values of properties of <paramref name="object"/>.
    /// </summary>
    /// <remarks>
    /// This method assumes that any token or parameter in the template to be replaced with values from <paramref name="object"/>'s properties is prefixed with the '<c>$</c>' character.
    /// </remarks>
    /// <typeparam name="T">The specific type of <paramref name="object"/>.</typeparam>
    /// <param name="template">The string to use as template.</param>
    /// <param name="object">The object whose properties'values will be used as values for the template.</param>
    /// <returns>
    /// <para>
    /// A string value from the <paramref name="template"/> with values or tokens replaced with values from <paramref name="object"/>'s properties.
    /// </para>
    /// <para>
    /// If <paramref name="object"/> is <see langword="null"/>, then this method returns the same value as <paramref name="template"/> without any token or parameter replacement.
    /// </para>
    /// </returns>
    public static string TemplateStringFormatter<T>(this string template, T @object)
    {
        return template.TemplateStringFormatter(@"$", @object);
    }

    /// <summary>
    /// Formats a string as template with tokens or parameters provided from the values of properties of <paramref name="object"/>.
    /// </summary>
    /// <typeparam name="T">The specific type of <paramref name="object"/>.</typeparam>
    /// <param name="template">The string to use as template.</param>
    /// <param name="templateObjectPropertiesPrefix">
    /// A value that identifies any token or parameter in the template to be replaced with values from <paramref name="object"/>'s properties is prefixed with the '<c>$</c>' character.
    /// </param>
    /// <param name="object">The object whose properties'values will be used as values for the template.</param>
    /// <returns>
    /// <para>
    /// A string value from the <paramref name="template"/> with values or tokens replaced with values from <paramref name="object"/>'s properties.
    /// </para>
    /// <para>
    /// If <paramref name="object"/> is <see langword="null"/>, then this method returns the same value as <paramref name="template"/> without any token or parameter replacement.
    /// </para>
    /// </returns>
    public static string TemplateStringFormatter<T>(this string template, string templateObjectPropertiesPrefix, T @object)
    {
        return template.TemplateStringFormatterWithValues(@object?.ToPropertyDictionary(templateObjectPropertiesPrefix));
    }

    /// <summary>
    /// Formats a string as template with tokens or parameters provided from the values in <paramref name="templateValues"/>.
    /// </summary>
    /// <param name="template">The string to use as template.</param>
    /// <param name="templateValues">A dictionalry with values to use as replacements for tokens or parameters in <paramref name="template"/>.</param>
    /// <returns>
    /// <para>
    /// A string value from the <paramref name="template"/> with values or tokens replaced with values from <paramref name="templateValues"/>'s properties.
    /// </para>
    /// <para>
    /// If <paramref name="templateValues"/> is <see langword="null"/>, then this method returns the same value as <paramref name="template"/> without any token or parameter replacement.
    /// </para>
    /// </returns>
    public static string TemplateStringFormatterWithValues(this string template, IDictionary<string, object> templateValues)
    {
        return templateValues == null
            ? template
            : templateValues.Aggregate(template, (current, parameter) => current.Replace(parameter.Key, parameter.Value?.ToString(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Validates if a given string value corresponds to a valid e-mail address format.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="System.Text.RegularExpressions">Regular Expressions</see> to process the <paramref name="email">e-mail</paramref>.
    /// It is very important to provide a timeout interval to prevent any untrusted input from a malicious user from causing a «Denial of Service» attack.
    /// </remarks>
    /// <param name="email">The string value to validate as an e-mail address.</param>
    /// <returns>Returns <see langword="true"/> if the string contains a valid e-mail address, otherwise returns <see langword="false"/>.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format"/>
    public static bool IsValidEmail(this string email)
    {
        return IsValidEmail(email, 250);
    }

    /// <summary>
    /// Validates if a given string value corresponds to a valid e-mail address format.
    /// </summary>
    /// <remarks>
    /// This method uses <see cref="System.Text.RegularExpressions">Regular Expressions</see> to process the <paramref name="email">e-mail</paramref>.
    /// It is very important to provide a timeout interval to prevent any untrusted input from a malicious user from causing a «Denial of Service» attack.
    /// </remarks>
    /// <param name="email">The string value to validate as an e-mail address.</param>
    /// <param name="matchCheckTimeoutMilliseconds">A timeout interval in milliseconds to indicate when this method should time out. By default it is 250 milliseconds.</param>
    /// <returns>Returns <see langword="true"/> if the string contains a valid e-mail address, otherwise returns <see langword="false"/>.</returns>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format"/>
    public static bool IsValidEmail(this string email, double matchCheckTimeoutMilliseconds)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(matchCheckTimeoutMilliseconds));

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(matchCheckTimeoutMilliseconds));

            // Examines the domain part of the email and normalizes it.
            static string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
