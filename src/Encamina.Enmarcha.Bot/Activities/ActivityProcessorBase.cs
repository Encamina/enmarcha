using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.Bot.Abstractions.Activities;

using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Activities;

/// <summary>
/// Base class for processing <see cref="Activity">activities</see> from a <see cref="ITurnContext">turn context</see>.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class ActivityProcessorBase : HandlerManagerBase<IActivityHandler>, IActivityProcessor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityProcessorBase"/> class.
    /// </summary>
    /// <param name="handlers">A collection of handlers for this manager.</param>
    protected ActivityProcessorBase(IEnumerable<IActivityHandler> handlers) : base(handlers)
    {
    }

    /// <inheritdoc/>
    public async Task<bool> BeginProcessAsync(IActivity activity, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        return await ProcessAsync(activity, turnContext, HandlerProcessTimes.Begin, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> EndProcessAsync(IActivity activity, ITurnContext turnContext, CancellationToken cancellationToken)
    {
        return await ProcessAsync(activity, turnContext, HandlerProcessTimes.End, cancellationToken);
    }

    private async Task<bool> ProcessAsync(IActivity activity, ITurnContext turnContext, HandlerProcessTimes processTime, CancellationToken cancellationToken)
    {
        var handlers = Handlers?.Where(h => h.ProcessTime.HasFlag(processTime) || h.ProcessTime.HasFlag(HandlerProcessTimes.Both)).OrderBy(h => h.Order);

        if (handlers != null)
        {
            foreach (var handler in handlers)
            {
                if (await handler.HandleAsync(activity, turnContext, cancellationToken))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
