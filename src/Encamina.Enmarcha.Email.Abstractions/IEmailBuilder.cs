using System.Net.Mime;
using System.Text;

namespace Encamina.Enmarcha.Email.Abstractions;

#pragma warning disable S2360 // Optional parameters should not be used

/// <summary>
/// Represents a builder that allows creating and sending a new e-mail.
/// </summary>
public interface IEmailBuilder
{
    /// <summary>
    /// Gets the (currently) build e-mail agnostic specification.
    /// </summary>
    EmailSpecification Specification { get; }

    /// <summary>
    /// Sets the e-mail address of the sender of an e-mail, typically the 'FROM' field of an e-mail.
    /// </summary>
    /// <param name="emailAddress">The sender's e-mail address, which will also be used as the sender's name.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetSender(string emailAddress);

    /// <summary>
    /// Sets the e-mail address of the sender of an e-mail, typically the 'FROM' field of an e-mail.
    /// </summary>
    /// <param name="emailAddress">The sender's e-mail address.</param>
    /// <param name="senderName">
    /// The senders's name. If <see langword="null"/> or <see cref="string.Empty"/>, then <paramref name="emailAddress"/> will be used as sender's name.
    /// </param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetSender(string emailAddress, string senderName);

    /// <summary>
    /// Sets as sender the e-mail address configured in the SMTP options as the SMTP user account, with
    /// an optional custom sender name.
    /// </summary>
    /// <param name="senderName">Optional (custom) sender name.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetDefaultSender(string senderName = null);

    /// <summary>
    /// Adds an attachment.
    /// </summary>
    /// <param name="fileName">The file name of the attachment.</param>
    /// <param name="data">The binary data of the attachment.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder AddAttachment(string fileName, byte[] data);

    /// <summary>
    /// Adds an attachment given a content type description.
    /// </summary>
    /// <param name="fileName">The file name of the attachment.</param>
    /// <param name="data">The binary data of the attachment.</param>
    /// <param name="contentTypeValue">The content type name or description.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder AddAttachment(string fileName, byte[] data, string contentTypeValue);

    /// <summary>
    /// Adds an attachment given a specific <see cref="ContentType">content type</see>.
    /// </summary>
    /// <param name="fileName">The file name of the attachment.</param>
    /// <param name="data">The binary data of the attachment.</param>
    /// <param name="contentType">The content type for the attachment.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder AddAttachment(string fileName, byte[] data, ContentType contentType);

    /// <summary>
    /// Adds an e-mail address for a recipient of an e-mail, typically the 'TO' field of an e-mail.
    /// </summary>
    /// <param name="emailAddress">The recipients's e-mail address.</param>
    /// <param name="recipientName">The recipient's name. Defaults to <see langword="null"/>.</param>
    /// <param name="recipientType">The type of recipient (like 'to', 'cc', or 'bcc'). Defaults to <see cref="EmailRecipientType.TO"/>.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder AddRecipient(string emailAddress, string recipientName = null, EmailRecipientType recipientType = EmailRecipientType.TO);

    /// <summary>
    /// Sets the e-mail's subject.
    /// </summary>
    /// <param name="subject">The e-mail's subject.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetSubject(string subject);

    /// <summary>
    /// Sets the e-mails body.
    /// </summary>
    /// <param name="body">The e-mail's body.</param>
    /// <param name="isHtml">A flag to indicate whether the body of the e-mail is an HTML or not. Default is <see langword="false"/>.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetBody(string body, bool isHtml = false);

    /// <summary>
    /// Sets the e-mail's body from a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="body">An <see cref="StringBuilder"/> used to build the e-mail's body.</param>
    /// <param name="isHtml">A flag to indicate whether the body of the e-mail is an HTML or not. Default is <see langword="false"/>.</param>
    /// <returns>The <see cref="IEmailBuilder"/> so that additional calls can be chained.</returns>
    IEmailBuilder SetBody(StringBuilder body, bool isHtml = false);

    /// <summary>
    /// Sends the e-mail, thus effectively ending its building.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    Task SendAsync(CancellationToken cancellationToken);
}

#pragma warning restore S2360 // Optional parameters should not be used
