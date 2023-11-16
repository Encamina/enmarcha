namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents a provider to access the current SMTP client options.
/// </summary>
public interface ISmtpClientOptionsProvider
{
    /// <summary>
    /// Gets the current SMTP client options.
    /// </summary>
    SmtpClientOptions SmtpClientOptions { get; }
}
