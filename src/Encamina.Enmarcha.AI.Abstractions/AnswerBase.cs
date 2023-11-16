using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base class for answers from a question.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class AnswerBase : IdentifiableBase<string>, IAnswer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnswerBase"/> class.
    /// </summary>
    protected AnswerBase()
    {
    }

    /// <inheritdoc/>
    public virtual string Value { get; init; }

    /// <inheritdoc/>
    public virtual double? ConfidenceScore { get; init; }

    /// <inheritdoc/>
    public virtual string Source { get; init; }

    /// <inheritdoc/>
    public virtual IReadOnlyList<string> AssociatedQuestions { get; init; }

    /// <inheritdoc/>
    public virtual IReadOnlyDictionary<string, string> Metadata { get; init; }

    /// <inheritdoc/>
    public bool Equals(IAnswer x, IAnswer y) => (x?.Id == null && y?.Id == null) || (x?.Id != null && y?.Id != null && string.Equals(x.Id, y.Id));

    /// <inheritdoc/>
    public int GetHashCode(IAnswer obj) => obj != null ? obj.GetHashCode() : 0;
}
