using System.Globalization;
using System.Resources;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for a <see cref="ResourceManager"/>.
/// </summary>
public static class ResourceManagerExtensions
{
    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.CurrentCulture"/>, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetStringByCurrentCulture(this ResourceManager resourceManager, string resourceName)
    {
        return resourceManager.GetString(resourceName, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.CurrentUICulture"/>.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.CurrentUICulture"/>, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetStringByCurrentUICulture(this ResourceManager resourceManager, string resourceName)
    {
        return resourceManager.GetString(resourceName, CultureInfo.CurrentUICulture);
    }

    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.InvariantCulture"/>, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetStringByInvariantCulture(this ResourceManager resourceManager, string resourceName)
    {
        return resourceManager.GetString(resourceName, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.CurrentCulture"/> formatted by replacing
    /// items in it with the string representations of corresponding objects in a specified array.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.CurrentCulture"/> and formatted with replaced
    /// items from the given array of objects, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetFormattedStringByCurrentCulture(this ResourceManager resourceManager, string resourceName, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentCulture, resourceManager.GetStringByCurrentCulture(resourceName), args);
    }

    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.CurrentUICulture"/> formatted by replacing
    /// items in it with the string representations of corresponding objects in a specified array.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.CurrentUICulture"/> and formatted with replaced
    /// items from the given array of objects, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetFormattedStringByCurrentUICulture(this ResourceManager resourceManager, string resourceName, params object?[] args)
    {
        return string.Format(CultureInfo.CurrentUICulture, resourceManager.GetStringByCurrentUICulture(resourceName), args);
    }

    /// <summary>
    /// Gets the value of the string resource localized from <see cref="CultureInfo.InvariantCulture"/> formatted by replacing
    /// items in it with the string representations of corresponding objects in a specified array.
    /// </summary>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="resourceName">The name of the resource to retrieve.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <returns>
    /// The value of the resource localized from <see cref="CultureInfo.InvariantCulture"/> and formatted with replaced
    /// items from the given array of objects, or <see langword="null"/> if it cannot be found in a resource set.
    /// </returns>
    public static string GetFormattedStringByInvariantCulture(this ResourceManager resourceManager, string resourceName, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, resourceManager.GetStringByInvariantCulture(resourceName), args);
    }
}
