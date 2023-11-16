using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents a provider to build and send e-mails.
/// </summary>
public interface IEmailProvider : INameable
{
    /// <summary>
    /// Creates an email builder to begin sending an email.
    /// </summary>
    /// <returns>
    /// A valid instance of an <see cref="IEmailBuilder"/> to start building the e-mail to send.
    /// </returns>
    IEmailBuilder BeginSendEmail();
}
