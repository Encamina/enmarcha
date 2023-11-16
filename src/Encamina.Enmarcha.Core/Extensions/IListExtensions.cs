using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension helper methods for an <see cref="ICollection{T}"/>.
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Adds the elements of the specified collection to the end of the <see cref="IList{T}"/>.
    /// </summary>
    /// <remarks>
    /// This extension method was created to provide the quite usefule <c>AddRange</c> to instances that are
    /// obtained as <see cref="IList{T}"/> instead of <see cref="List{T}"/> and minimize the need to create
    /// a new object.
    /// </remarks>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="destination">The list to add items to.</param>
    /// <param name="source">
    /// The collection whose elements should be added to the end of the <see cref="IList{T}"/>. The collection itself cannot be <see langword="null"/>,
    /// but it can contain elements that are <see langword="null"/>, if type <typeparamref name="T"/> is a reference type.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static void AddRange<T>(this IList<T> destination, IEnumerable<T> source)
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
