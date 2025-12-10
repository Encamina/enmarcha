using System;
using System.Collections.Generic;
using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension helper methods for an <see cref="ICollection{T}"/>.
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collections.</typeparam>
    /// <param name="destination">The collection to add items to.</param>
    /// <param name="source">
    /// The collection whose elements should be added to the end of the <see cref="ICollection{T}"/>. The collection itself cannot be <see langword="null"/>,
    /// but it can contain elements that are <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
    {
        Guard.IsNotNull(source);

        if (destination is List<T> list)
        {
            list.AddRange(source);
        }
        else
        {
            foreach (var item in source)
            {
                destination.Add(item);
            }
        }
    }
}
