using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension helper methods for an <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    /// Merges two dictionaries in-place, putting items from the <paramref name="right"/> dictionary into the <paramref name="left"/> dictionary.
    /// </summary>
    /// <remarks>
    /// When duplicate keys exist, this method keeps the value from the <paramref name="right"/> (instead of throwing an exception, or keeping the value from the <paramref name="left"/>).
    /// </remarks>
    /// <typeparam name="TKey">The type for the dictionary's keys.</typeparam>
    /// <typeparam name="TValue">The type for the dictionary's values.</typeparam>
    /// <param name="left">The dictionary to add items into.</param>
    /// <param name="right">
    /// The dictionary whose elements should be added to the <paramref name="left"/> dictionary. The dictionary itself should not be <see langword="null"/>,
    /// but it can contain elements that are <see langword="null"/>, if type <typeparamref name="TValue"/> is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="left"/> is <see langword="null"/>.
    /// </exception>
    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
    {
        InnerMerge(left, right);
    }

    /// <summary>
    /// Merges a dictionaries in-place with values from a read only dictionary, putting items from the <paramref name="right"/> read only dictionary into the <paramref name="left"/> dictionary.
    /// </summary>
    /// <remarks>
    /// When duplicate keys exist, this method keeps the value from the <paramref name="right"/> (instead of throwing an exception, or keeping the value from the <paramref name="left"/>).
    /// </remarks>
    /// <typeparam name="TKey">The type for the dictionary's keys.</typeparam>
    /// <typeparam name="TValue">The type for the dictionary's values.</typeparam>
    /// <param name="left">The dictionary to add items into.</param>
    /// <param name="right">
    /// The read only dictionary whose elements should be added to the <paramref name="left"/> dictionary. The dictionary itself should not be <see langword="null"/>,
    /// but it can contain elements that are <see langword="null"/>, if type <typeparamref name="TValue"/> is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="left"/> is <see langword="null"/>.
    /// </exception>
    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> left, IReadOnlyDictionary<TKey, TValue> right)
    {
        InnerMerge(left, right);
    }

    private static void InnerMerge<TKey, TValue>(IDictionary<TKey, TValue> left, IEnumerable<KeyValuePair<TKey, TValue>> right)
    {
        Guard.IsNotNull(left);

        if (right != null)
        {
            foreach (var item in right)
            {
                left[item.Key] = item.Value;
            }
        }
    }
}
