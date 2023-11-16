using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Entities;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.Email.MailKit;

/// <summary>
/// Factory that provides valid instances of <see cref="IEmailProvider"/>s.
/// </summary>
internal sealed class EmailServiceFactory : ServiceFactory<IEmailProvider>, IEmailProviderFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailServiceFactory"/> class.
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: This class implements a «Service Locator» pattern.
    /// </remarks>
    /// <param name="serviceScope">A service scope from which to retrieve <see cref="IEmailProvider"/>s.</param>
    public EmailServiceFactory(IServiceScope serviceScope) : base(serviceScope)
    {
    }
}
