namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="object"/>s.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Creates a dictionary from values in an <paramref name="object"/>'s properties.
    /// </summary>
    /// <param name="object">The object.</param>
    /// <returns>
    /// A dictionary with values from <paramref name="object"/>'s properties.
    /// </returns>
    public static IDictionary<string, object> ToPropertyDictionary(this object @object)
    {
        return @object.ToPropertyDictionary(null);
    }

    /// <summary>
    /// Creates a dictionary from values in an <paramref name="object"/>'s properties.
    /// Each dictionary's key will be prefixed with given <paramref name="keyPrefix"/> value.
    /// </summary>
    /// <param name="object">The object.</param>
    /// <param name="keyPrefix">A prefix value to use with each key of the returned dictionary.</param>
    /// <returns>
    /// A dictionary with values from <paramref name="object"/>'s properties.
    /// </returns>
    public static IDictionary<string, object> ToPropertyDictionary(this object @object, string? keyPrefix)
    {
        var dictionary = new Dictionary<string, object>();

        var prefix = string.IsNullOrWhiteSpace(keyPrefix) ? string.Empty : keyPrefix.Trim();

        foreach (var propertyInfo in @object.GetType().GetProperties())
        {
            if (propertyInfo.CanRead && propertyInfo.GetIndexParameters().Length == 0)
            {
                dictionary[prefix + propertyInfo.Name] = propertyInfo.GetValue(@object, null);
            }
        }

        return dictionary;
    }

    /// <summary>
    /// Creates a dictionary from values in an <paramref name="object"/>'s properties.
    /// Each dictionary's key will be prefixed with the name of <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This extension method is quite useful to create in-memory configurations.</remarks>
    /// <typeparam name="T">The type the object from which the dictionary will be created.</typeparam>
    /// <param name="object">The object.</param>
    /// <returns>
    /// A dictionary with values from <paramref name="object"/>'s properties as strings.
    /// </returns>
    public static IDictionary<string, string> ToPropertyDictionary<T>(this T @object)
    {
        var dictionary = new Dictionary<string, string>();
        var propertiesDictionary = @object.ToPropertyDictionary($@"{typeof(T).Name}:");

        foreach (var property in propertiesDictionary)
        {
            if (property.Value is IDictionary<string, string> innerProperties)
            {
                foreach (var innerProperty in innerProperties)
                {
                    dictionary.Add($@"{property.Key}:{innerProperty.Key}", innerProperty.Value);
                }
            }
            else
            {
                dictionary.Add(property.Key, property.Value?.ToString());
            }
        }

        return dictionary;
    }
}
