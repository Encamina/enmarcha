using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a handler for metadata.
/// </summary>
public interface IMetadataHandler : IOrderable
{
    /// <summary>
    /// Handles metadata from a message with given options.
    /// </summary>
    /// <param name="message">A message from which to obtain or handle metadata.</param>
    /// <param name="currentMetadataOptions">Current metadata options that might have come from other handlers.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>An instance of <see cref="MetadataOptions"/> with values from the handled metadata.</returns>
    Task<MetadataOptions> HandleMessageAsync(string message, MetadataOptions currentMetadataOptions, CancellationToken cancellationToken);

    /// <summary>
    /// Handles answers using given metadata options, usually to filter of modifying them.
    /// </summary>
    /// <param name="answers">A collection of answers.</param>
    /// <param name="metadataOptions">Current metadata options that may have come from other handlers.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The collection of answets after handling the metadata with given options.</returns>
    Task<IReadOnlyCollection<IAnswer>> HandleAnswersAsync(IEnumerable<IAnswer> answers, MetadataOptions metadataOptions, CancellationToken cancellationToken);
}
