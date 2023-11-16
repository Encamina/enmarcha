using Encamina.Enmarcha.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions.Internals;

internal sealed class CompletionServiceFactory : ServiceFactory<ICompletionService>, ICompletionServiceFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionServiceFactory"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>IMPORTANT</b>: This class is disposable. Please dispose it after use.
    /// </para>
    /// <para>
    /// <b>WARNING</b>: This class implements a «Service Locator» pattern.
    /// </para>
    /// </remarks>
    /// <param name="serviceScope">A service scope from which to retrieve <see cref="ICompletionService"/>s.</param>
    public CompletionServiceFactory(IServiceScope serviceScope) : base(serviceScope)
    {
    }
}
