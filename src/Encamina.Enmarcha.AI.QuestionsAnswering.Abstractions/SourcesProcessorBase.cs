using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Base class for sources handler processor.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class SourcesProcessorBase : OrderableHandlerManagerBase<ISourcesHandler>, ISourcesProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SourcesProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of sources handlers for this processor.</param>
    protected SourcesProcessorBase(IEnumerable<ISourcesHandler> handlers) : base(handlers)
    {
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<IAnswer>> ProcessAnswersAsync(IEnumerable<IAnswer> answers, IEnumerable<string> sources, CancellationToken cancellationToken)
    {
        var processedAnswers = new List<IAnswer>();

        if (Handlers?.Any() ?? false)
        {
            foreach (var handler in Handlers)
            {
                processedAnswers.AddRange(await handler.HandleAnswersAsync(answers, sources, cancellationToken));
            }
        }

        return processedAnswers.Distinct().ToArray();
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<string>> ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        var sources = new List<string>();

        if (Handlers?.Any() ?? false)
        {
            foreach (var handler in Handlers)
            {
                sources.AddRange(await handler.HandleMessageAsync(message, cancellationToken));
            }
        }

        return sources;
    }
}
