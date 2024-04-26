using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Encamina.Enmarcha.AspNet.Mvc.Formatters.Csv;

internal static class Helpers
{
    internal static bool IsTypeOfIEnumerable(Type type)
    {
        return type.GetInterfaces().Any(i => i == typeof(IList) || i == typeof(IEnumerable));
    }

    internal static string GetDisplayName(PropertyInfo property)
    {
        var attrName = GetAttributeDisplayName(property);

        return string.IsNullOrWhiteSpace(attrName) ? property.Name : attrName;
    }

    internal static string? GetAttributeDisplayName(PropertyInfo property)
    {
        var atts = property.GetCustomAttributes(typeof(DisplayNameAttribute), true);

        return atts.Length == 0 ? null : (atts[0] as DisplayNameAttribute).DisplayName;
    }
}
