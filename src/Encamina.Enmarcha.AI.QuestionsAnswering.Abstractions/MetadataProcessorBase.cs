using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Base class for metadata handlers processor.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class MetadataProcessorBase : OrderableHandlerManagerBase<IMetadataHandler>, IMetadataProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MetadataProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of metadata handlers for this processor.</param>
    protected MetadataProcessorBase(IEnumerable<IMetadataHandler> handlers) : base(handlers)
    {
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyCollection<IAnswer>> ProcessAnswersAsync(IEnumerable<IAnswer> answers, MetadataOptions metadataOptions, CancellationToken cancellationToken)
    {
        var processedAnswers = new List<IAnswer>();

        if (Handlers?.Any() ?? false)
        {
            foreach (var handler in Handlers)
            {
                processedAnswers.AddRange(await handler.HandleAnswersAsync(answers, metadataOptions, cancellationToken));
            }
        }

        return processedAnswers.Distinct().ToArray();
    }

    /// <inheritdoc/>
    public virtual Task<MetadataOptions> ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        return ProcessMessageAsync(message, null, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<MetadataOptions> ProcessMessageAsync(string message, MetadataOptions metadataOptions, CancellationToken cancellationToken)
    {
        metadataOptions ??= new MetadataOptions();

        if (Handlers?.Any() ?? false)
        {
            foreach (var handler in Handlers)
            {
                metadataOptions = await handler.HandleMessageAsync(message, metadataOptions, cancellationToken);
            }
        }

        return metadataOptions;
    }
}
