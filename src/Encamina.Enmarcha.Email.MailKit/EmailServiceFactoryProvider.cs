using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Entities;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Email.MailKit;

/// <summary>
/// A provider for factories of <see cref="IEmailProvider"/>s.
/// </summary>
internal sealed class EmailServiceFactoryProvider : ServiceFactoryProvider<IEmailProvider>, IEmailProviderFactoryProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailServiceFactoryProvider"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">
    /// A factory to create instances of <see cref="IServiceScope"/>, required to create <see cref="IEmailProvider"/>s. within a scope.
    /// </param>
    public EmailServiceFactoryProvider(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailServiceFactoryProvider"/> class.
    /// </summary>
    /// <param name="serviceScopeFactory">
    /// A factory to create instances of <see cref="IServiceScope"/>, required to create <see cref="IEmailProvider"/>s. within a scope.
    /// </param>
    /// <param name="serviceFactoryBuilder">A builder for <see cref="IServiceFactory{IEmailProvider}"/> factories.</param>
    public EmailServiceFactoryProvider(IServiceScopeFactory serviceScopeFactory, Func<IServiceScopeFactory, IServiceFactory<IEmailProvider>> serviceFactoryBuilder) : base(serviceScopeFactory, serviceFactoryBuilder)
    {
    }
}
