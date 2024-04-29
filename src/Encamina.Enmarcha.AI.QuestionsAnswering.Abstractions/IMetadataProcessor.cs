namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a processor for metadata handlers.
/// </summary>
public interface IMetadataProcessor
{
    /// <summary>
    /// Gets the collection of metadata handlers.
    /// </summary>
    IEnumerable<IMetadataHandler> Handlers { get; init; }

    /// <summary>
    /// Process a collection of answers with given metadata options, usually to filter or modifying them.
    /// </summary>
    /// <param name="answers">A collection of answers to process with given metadata options.</param>
    /// <param name="metadataOptions">The metadata options to use when processing the answers.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>
    /// A read only collection of answers processed with given metadata options.
    /// </returns>
    Task<IReadOnlyCollection<IAnswer>> ProcessAnswersAsync(IEnumerable<IAnswer> answers, MetadataOptions metadataOptions, CancellationToken cancellationToken);

    /// <summary>
    /// Process a message to obtain metadata options from it.
    /// </summary>
    /// <param name="message">The message to process for metadata.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A valid instance of <see cref="MetadataOptions"/> of metadata is successfully retrieved from the given message, otherwise <see langword="null"/>.</returns>
    Task<MetadataOptions> ProcessMessageAsync(string message, CancellationToken cancellationToken);

    /// <summary>
    /// Process a message to obtain metadata options from it.
    /// </summary>
    /// <param name="message">The message to process for metadata.</param>
    /// <param name="metadataOptions">The metadata options to use when processing the message.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A valid instance of <see cref="MetadataOptions"/> of metadata is successfully retrieved from the given message, otherwise <see langword="null"/>.</returns>
    Task<MetadataOptions> ProcessMessageAsync(string message, MetadataOptions? metadataOptions, CancellationToken cancellationToken);
}