using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents a provider for factories of <see cref="IEmailProvider"/>s.
/// </summary>
public interface IEmailProviderFactoryProvider : IServiceFactoryProvider<IEmailProvider>
{
}
