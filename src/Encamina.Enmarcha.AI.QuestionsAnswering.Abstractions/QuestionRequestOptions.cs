namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Options for a question request.
/// </summary>
public class QuestionRequestOptions : IQuestionRequestOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionRequestOptions"/> class.
    /// </summary>
    public QuestionRequestOptions()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionRequestOptions"/> class with given sources.
    /// </summary>
    /// <param name="sources">A collection of sources to initialize this question request options.</param>
    public QuestionRequestOptions(IEnumerable<string> sources)
    {
        Sources = sources as IReadOnlyCollection<string> ?? sources.ToList().AsReadOnly();
    }

    /// <inheritdoc/>
    public virtual IReadOnlyCollection<string> Sources { get; } = Array.Empty<string>();

    /// <inheritdoc/>
    public virtual MetadataOptions MetadataOptions { get; init; }
}
