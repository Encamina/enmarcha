using System.Net.Mail;
using System.Net.Mime;
using System.Text;

using Encamina.Enmarcha.Email.Abstractions;

using MimeKit;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// Represents a raw SMTP message.
/// </summary>
internal sealed class RawSmtpMessage
{
    private readonly List<EmailRecipientSpecification> recipients = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RawSmtpMessage"/> class.
    /// </summary>
    internal RawSmtpMessage()
    {
        Raw = new StringBuilder();
    }

    /// <summary>
    /// Gets the raw (data) of this SMTP message.
    /// </summary>
    internal StringBuilder Raw { get; init; }

    /// <summary>
    /// Adds a recipient email address to this raw SMTP message.
    /// </summary>
    /// <param name="recipient">The recipient email specification to add.</param>
    internal void AddRecipient(EmailRecipientSpecification recipient)
    {
        recipients.Add(recipient);
    }

    /// <summary>
    /// Gets this SMTP message as a valid instance of <see cref="MailMessage"/>.
    /// </summary>
    /// <returns>A valid instance of <see cref="MailMessage"/> from this SMTP message raw data.</returns>
    internal MailMessage AsMailMessage()
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Raw.ToString().TrimEnd('\r', '\n')));
        using var mimeMessage = MimeMessage.Load(memoryStream);

        var msg = new MailMessage();

        var from = mimeMessage.From.Mailboxes.FirstOrDefault();
        if (from != null)
        {
            msg.From = ConvertToMailAddress(from);
        }

        var sender = mimeMessage.Sender;
        if (sender != null)
        {
            msg.Sender = ConvertToMailAddress(sender);
        }

        if (mimeMessage.Body != null)
        {
            AddBodyPart(msg, mimeMessage.Body);
        }

        foreach (var header in mimeMessage.Headers)
        {
            if (!string.IsNullOrEmpty(header.Value))
            {
                msg.Headers.Add(header.Field, header.Value);
            }
        }

        foreach (var mailbox in mimeMessage.ReplyTo.Mailboxes)
        {
            msg.ReplyToList.Add(ConvertToMailAddress(mailbox));
        }

        foreach (var mailbox in mimeMessage.To.Mailboxes)
        {
            msg.To.Add(ConvertToMailAddress(mailbox));
        }

        foreach (var mailbox in mimeMessage.Cc.Mailboxes)
        {
            msg.CC.Add(ConvertToMailAddress(mailbox));
        }

        foreach (var mailbox in mimeMessage.Bcc.Mailboxes)
        {
            msg.Bcc.Add(ConvertToMailAddress(mailbox));
        }

        msg.Subject = mimeMessage.Subject;

        return msg;
    }

    private static void AddBodyPart(MailMessage message, MimeEntity entity)
    {
        if (entity is Multipart multipart)
        {
            ManageMultipart(message, multipart);
        }
        else
        {
            if (entity is MimePart part)
            {
                ManageMimePart(message, part);
            }
        }
    }

    private static void ManageMultipart(MailMessage message, Multipart multipart)
    {
        if (multipart.ContentType.IsMimeType(@"multipart", @"alternative"))
        {
            foreach (var part in multipart.OfType<MimePart>())
            {
                // clone the content
                var content = new MemoryStream();
                part.Content.DecodeTo(content);
                content.Position = 0;

                var view = new AlternateView(content, GetContentType(part.ContentType))
                {
                    TransferEncoding = GetTransferEncoding(part.ContentTransferEncoding),
                };

                if (!string.IsNullOrEmpty(part.ContentId))
                {
                    view.ContentId = part.ContentId;
                }

                message.AlternateViews.Add(view);
            }
        }
        else
        {
            foreach (var part in multipart)
            {
                AddBodyPart(message, part);
            }
        }
    }

    private static void ManageMimePart(MailMessage message, MimePart mimePart)
    {
        if (mimePart.IsAttachment || !string.IsNullOrEmpty(message.Body) || mimePart is not TextPart)
        {
            // Clone the content
            var content = new MemoryStream();
            mimePart.Content.DecodeTo(content);
            content.Position = 0;

            var attachment = new Attachment(content, GetContentType(mimePart.ContentType));

            if (mimePart.ContentDisposition != null)
            {
                attachment.ContentDisposition.DispositionType = mimePart.ContentDisposition.Disposition;

                foreach (var param in mimePart.ContentDisposition.Parameters)
                {
                    attachment.ContentDisposition.Parameters.Add(param.Name, param.Value);
                }
            }

            attachment.TransferEncoding = GetTransferEncoding(mimePart.ContentTransferEncoding);

            if (!string.IsNullOrEmpty(mimePart.ContentId))
            {
                attachment.ContentId = mimePart.ContentId;
            }

            message.Attachments.Add(attachment);
        }
        else
        {
            message.IsBodyHtml = mimePart.ContentType.IsMimeType(@"text", @"html");
            message.Body = ((TextPart)mimePart).Text;
        }
    }

    private static System.Net.Mime.ContentType GetContentType(MimeKit.ContentType contentType)
    {
        var ctype = new System.Net.Mime.ContentType
        {
            MediaType = $@"{contentType.MediaType}/{contentType.MediaSubtype}",
        };

        foreach (var parameter in contentType.Parameters)
        {
            ctype.Parameters.Add(parameter.Name, parameter.Value);
        }

        return ctype;
    }

    private static TransferEncoding GetTransferEncoding(ContentEncoding encoding)
    {
        return encoding switch
        {
            ContentEncoding.EightBit or ContentEncoding.QuotedPrintable => TransferEncoding.QuotedPrintable,
            ContentEncoding.SevenBit => TransferEncoding.SevenBit,
            _ => TransferEncoding.Base64,
        };
    }

    private static MailAddress ConvertToMailAddress(MailboxAddress address)
    {
        return new MailAddress(address.Address, address.Name ?? string.Empty, address.Encoding ?? Encoding.UTF8);
    }
}
