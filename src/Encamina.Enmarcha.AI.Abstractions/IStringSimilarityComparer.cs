namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Provides methods for comparing the similarity of two strings.
/// </summary>
public interface IStringSimilarityComparer
{
    /// <summary>
    /// Compares the similarity of two strings.
    /// </summary>
    /// <param name="first">The first string to compare.</param>
    /// <param name="second">The second string to compare.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A value between 0 and 1 indicating the similarity of the two strings, where 0 means the strings are completely different and 1 means the strings are identical.</returns>
    Task<double> CompareAsync(string first, string second, CancellationToken cancellationToken);
}