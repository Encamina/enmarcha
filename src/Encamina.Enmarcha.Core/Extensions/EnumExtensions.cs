using System.ComponentModel;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extensions for enumeration types.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets a description from an enumeration value. This description is set using the <see cref="DescriptionAttribute"/>.
    /// </summary>
    /// <param name="enumValue">The enumeration value.</param>
    /// <returns>
    /// The description for the enumeration value as set with the <see cref="DescriptionAttribute"/>. If not, returns <see langword="null"/>.
    /// </returns>
    public static string GetEnumDescription(this Enum enumValue)
    {
        return enumValue.GetEnumDescription(false);
    }

    /// <summary>
    /// Gets a description from an enumeration value. This description is set using the <see cref="DescriptionAttribute"/>.
    /// </summary>
    /// <param name="enumValue">The enumeration value.</param>
    /// <param name="throwIfNoDescriptionFound">
    /// A value indicating whether an exception should be thrown or not if a description for the enumeration value could not be found.
    /// </param>
    /// <returns>
    /// The description for the enumeration value as set with the <see cref="DescriptionAttribute"/>. If not, returns <see langword="null"/> unless
    /// the <paramref name="throwIfNoDescriptionFound"/> is <see langword="true"/>, in which case this methods trows a <see cref="ArgumentException"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If <paramref name="throwIfNoDescriptionFound"/> is <see langword="true"/> and the enumeration value does not have a description set using the <see cref="DescriptionAttribute"/>.
    /// </exception>
    public static string GetEnumDescription(this Enum enumValue, bool throwIfNoDescriptionFound)
    {
        var field = enumValue.GetType().GetField(enumValue.ToString());

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }

        if (throwIfNoDescriptionFound)
        {
            throw new ArgumentException($@"Description for '{enumValue}' not found!", nameof(enumValue));
        }

        return null;
    }
}
