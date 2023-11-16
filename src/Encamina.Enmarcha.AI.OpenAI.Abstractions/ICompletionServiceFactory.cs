using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Represents a factory that can provide valid instances of a specific <see cref="ICompletionService"/> type.
/// </summary>
public interface ICompletionServiceFactory : IServiceFactory<ICompletionService>
{
}
