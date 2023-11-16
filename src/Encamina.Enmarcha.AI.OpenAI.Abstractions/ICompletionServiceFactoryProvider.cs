using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Represents a provider for factories of <see cref="ICompletionService"/>s.
/// </summary>
public interface ICompletionServiceFactoryProvider : IServiceFactoryProvider<ICompletionService>
{
}
