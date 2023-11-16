namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for an <see cref="ISet{T}"/>.
/// </summary>
public static class ISetExtensions
{
    /// <summary>
    /// Adds the item if not present in the set. Returns true if added, and false if item already in set.
    /// </summary>
    /// <typeparam name="T">The type of the item to add.</typeparam>
    /// <param name="set">The set to add the item to.</param>
    /// <param name="item">The item to add to the set if missing.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the item is added, otherwise returns <see langword="true"/>.
    /// </returns>
    public static bool AddIfMissing<T>(this ISet<T> set, T item)
    {
        return !set.Contains(item) && set.Add(item);
    }

    /// <summary>
    /// Adds any and all items missing in set to the set and returns the number of items added. Returns true if item was removed.
    /// </summary>
    /// <typeparam name="T">The type of the item to remove.</typeparam>
    /// <param name="set">The set to remove the item from.</param>
    /// <param name="item">The item to remove from the set if exists.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the item is removed, otherwise returns <see langword="true"/>.
    /// </returns>
    public static bool RemoveIfExists<T>(this ISet<T> set, T item)
    {
        return set.Contains(item) && set.Remove(item);
    }
}
