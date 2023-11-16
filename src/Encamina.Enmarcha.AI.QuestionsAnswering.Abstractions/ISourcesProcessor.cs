namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a processor for sources handler .
/// </summary>
public interface ISourcesProcessor
{
    /// <summary>
    /// Gets the collection of source handlers.
    /// </summary>
    IEnumerable<ISourcesHandler> Handlers { get; init; }

    /// <summary>
    /// Process a collection of answers with given metadata options, usually to filter or modifying them.
    /// </summary>
    /// <param name="answers">The collection of answers to handle.</param>
    /// <param name="sources">The collection of sources to use.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A read only collection of answers handled by using the given collection of sources.</returns>
    Task<IReadOnlyCollection<IAnswer>> ProcessAnswersAsync(IEnumerable<IAnswer> answers, IEnumerable<string> sources, CancellationToken cancellationToken);

    /// <summary>
    /// Process a message to obtain sources from it.
    /// </summary>
    /// <param name="message">The message to process for sources.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A collection of sources from the given message.</returns>
    Task<IEnumerable<string>> ProcessMessageAsync(string message, CancellationToken cancellationToken);
}