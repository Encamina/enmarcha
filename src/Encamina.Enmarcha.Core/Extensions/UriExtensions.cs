namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for handling <see cref="Uri"/>s.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Appends segments to an <see cref="Uri"/>.
    /// </summary>
    /// <param name="baseUri">The base <see cref="Uri"/>.</param>
    /// <param name="segments">The collection of segments to append.</param>
    /// <returns>The <see cref="Uri"/> with all <paramref name="segments">segments</paramref> appended.</returns>
    public static Uri Append(this Uri baseUri, params string[] segments)
    {
        return new Uri(string.Join('/', new[] { baseUri.AbsoluteUri.TrimEnd('/') }.Concat(segments.Select(s => s.Trim('/')))));
    }
}
