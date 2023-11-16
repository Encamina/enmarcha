using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a handler for sources.
/// </summary>
public interface ISourcesHandler : IOrderable
{
    /// <summary>
    /// Handles sources from a message.
    /// </summary>
    /// <param name="message">A message from which to obtain or handle sources.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A collection of sources from the given message.</returns>
    Task<IEnumerable<string>> HandleMessageAsync(string message, CancellationToken cancellationToken);

    /// <summary>
    /// Handles answers using given sources, usually to filter of modifying them.
    /// </summary>
    /// <param name="answers">The collection of answers to handle.</param>
    /// <param name="sources">The collection of sources to use.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A read only collection of answers handled by using the given collection of sources.</returns>
    Task<IReadOnlyCollection<IAnswer>> HandleAnswersAsync(IEnumerable<IAnswer> answers, IEnumerable<string> sources, CancellationToken cancellationToken);
}
