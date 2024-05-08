namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents options for a question request.
/// </summary>
public interface IQuestionRequestOptions
{
    /// <summary>
    /// Gets a collection of sources.
    /// </summary>
    IReadOnlyCollection<string> Sources { get; }

    /// <summary>
    /// Gets the metadata options.
    /// </summary>
    MetadataOptions? MetadataOptions { get; }
}
