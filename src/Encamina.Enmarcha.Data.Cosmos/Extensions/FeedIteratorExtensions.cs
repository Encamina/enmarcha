using System.Runtime.CompilerServices;

using Microsoft.Azure.Cosmos;

namespace Encamina.Enmarcha.Data.Cosmos.Extensions;

#pragma warning disable S2360 // Optional parameters should not be used

/// <summary>
/// Extension methods to work with <see cref="FeedIterator{T}"/>.
/// </summary>
public static class FeedIteratorExtensions
{
    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> from the <see cref="FeedIterator{T}"/>.
    /// </summary>
    /// <typeparam name="TModel">The specific type of the model entity.</typeparam>
    /// <param name="setIterator">The <see cref="FeedIterator{T}"/>.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> from the <see cref="FeedIterator{T}"/>.</returns>
    public static async IAsyncEnumerable<TModel> ToAsyncEnumerable<TModel>(this FeedIterator<TModel> setIterator, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        while (setIterator.HasMoreResults)
        {
            foreach (var item in await setIterator.ReadNextAsync(cancellationToken))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Asynchronously creates an <see cref="IEnumerable{T}"/> from the <see cref="FeedIterator{T}"/>.
    /// </summary>
    /// <typeparam name="TModel">The specific type of the model entity.</typeparam>
    /// <param name="setIterator">The <see cref="FeedIterator{T}"/>.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> from the <see cref="FeedIterator{T}"/>.</returns>
    public static async Task<IEnumerable<TModel>> ToEnumerableAsync<TModel>(this FeedIterator<TModel> setIterator, CancellationToken cancellationToken = default)
    {
        var result = new List<TModel>();

        while (setIterator.HasMoreResults)
        {
            foreach (var item in await setIterator.ReadNextAsync(cancellationToken))
            {
                result.Add(item);
            }
        }

        return result;
    }
}

#pragma warning restore S2360 // Optional parameters should not be used
