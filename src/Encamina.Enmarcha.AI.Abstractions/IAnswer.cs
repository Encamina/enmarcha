using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents an answer from a question.
/// </summary>
public interface IAnswer : IConfidenceScore, IIdentifiableValuable<string, string>, IEqualityComparer<IAnswer>
{
    /// <summary>
    /// Gets the (name) of the source of the answer.
    /// </summary>
    string Source { get; init; }

    /// <summary>
    /// Gets a collection of associated questions for this answer.
    /// </summary>
    IReadOnlyList<string> AssociatedQuestions { get; init; }

    /// <summary>
    /// Gets a dictionary of the metadata of this answer.
    /// </summary>
    IReadOnlyDictionary<string, string> Metadata { get; init; }
}
