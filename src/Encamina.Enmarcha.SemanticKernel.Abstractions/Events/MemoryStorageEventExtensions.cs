using System.ComponentModel;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions.Events;

/// <summary>
/// <see cref="IMemoryManager.MemoryStorageEvent"/> extensions.
/// </summary>
public static class MemoryStorageEventExtensions
{
    /// <summary>
    /// Gets the enum description.
    /// </summary>
    /// <param name="value"><see cref="MemoryStorageEventEnum"/> value.</param>
    /// <returns>Text description.</returns>
    public static string GetEnumDescription(this MemoryStorageEventEnum value)
    {
        var fi = value.GetType().GetField(value.ToString());

        return fi.GetCustomAttributes(typeof(DescriptionAttribute), false) is DescriptionAttribute[] attributes && attributes.Any()
            ? attributes.First().Description
            : value.ToString();
    }
}
