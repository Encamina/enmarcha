using System.Net.Mail;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// Represents an event data for received mail messages.
/// </summary>
internal sealed class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
    /// </summary>
    /// <param name="message">A mail message for this event.</param>
    internal MessageReceivedEventArgs(MailMessage message)
    {
        Message = message;
    }

    /// <summary>
    /// Gets the mail message associated with this event.
    /// </summary>
    internal MailMessage Message { get; init; }
}
