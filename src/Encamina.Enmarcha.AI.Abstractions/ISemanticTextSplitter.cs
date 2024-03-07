namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents a semantic text splitter, which splits a text into semantic chunks based on embeddings.
/// </summary>
public interface ISemanticTextSplitter
{
    /// <summary>
    /// Splits the input text based on semantic content.
    /// </summary>
    /// <param name="text">The input text to be split.</param>
    /// <param name="embeddingsGenerator">A function to generate embeddings for a list of strings.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A collection of text splits.</returns>
    Task<IEnumerable<string>> SplitAsync(string text, Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>> embeddingsGenerator, CancellationToken cancellationToken);
}
