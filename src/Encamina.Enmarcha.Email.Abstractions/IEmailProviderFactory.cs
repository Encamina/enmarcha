using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents a factory that can provide valid instances of a specific <see cref="IEmailProvider"/> type.
/// </summary>
public interface IEmailProviderFactory : IServiceFactory<IEmailProvider>
{
}
