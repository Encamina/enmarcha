using Encamina.Enmarcha.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions.Internals;

internal sealed class CompletionServiceFactoryProvider : ServiceFactoryProvider<ICompletionService>, ICompletionServiceFactoryProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompletionServiceFactoryProvider"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">
    /// A factory to get instances of <see cref="ICompletionServiceFactory"/>, which can be used to get instances of <see cref="ICompletionService"/>.
    /// </param>
    public CompletionServiceFactoryProvider(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory, ssf => new CompletionServiceFactory(ssf.CreateScope()))
    {
    }
}
