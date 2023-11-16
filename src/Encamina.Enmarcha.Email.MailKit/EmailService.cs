using System.Net;
using System.Text;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Email.Abstractions;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

using ContentType = System.Net.Mime.ContentType;

namespace Encamina.Enmarcha.Email.MailKit;

/// <summary>
/// Service that provides e-mail creation capabilities.
/// </summary>
internal sealed class EmailService : IEmailBuilder, IEmailProvider, ISmtpClientOptionsProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="smtpClientOptions">Options for SMTP client.</param>
    public EmailService(IOptions<SmtpClientOptions> smtpClientOptions)
    {
        SmtpClientOptions = ValidateOptions(smtpClientOptions?.Value);
    }

    /// <inheritdoc/>
    public EmailSpecification Specification { get; } = new();

    /// <inheritdoc/>
    public SmtpClientOptions SmtpClientOptions { get; }

    /// <inheritdoc/>
    public string Name => SmtpClientOptions.Name;

    /// <inheritdoc/>
    public IEmailBuilder BeginSendEmail() => this;

    /// <inheritdoc/>
    public IEmailBuilder AddAttachment(string fileName, byte[] data) => AddAttachment(fileName, data, string.Empty);

    /// <inheritdoc/>
    public IEmailBuilder AddAttachment(string fileName, byte[] data, string contentTypeValue)
        => AddAttachment(fileName, data, string.IsNullOrWhiteSpace(contentTypeValue) ? null : new ContentType(contentTypeValue));

    /// <inheritdoc/>
    public IEmailBuilder AddAttachment(string fileName, byte[] data, ContentType contentType)
    {
        Specification.Attachments.Add(new EmailAttachmentSpecification()
        {
            ContentType = contentType,
            Data = data,
            FileName = fileName?.Trim(),
        });

        return this;
    }

    /// <inheritdoc/>
    public IEmailBuilder AddRecipient(string emailAddress, string recipientName = null, EmailRecipientType recipientType = EmailRecipientType.TO)
    {
        Guard.IsNotNullOrWhiteSpace(emailAddress);

        Specification.To.Add(new EmailRecipientSpecification()
        {
            Address = emailAddress,
            Name = string.IsNullOrWhiteSpace(recipientName) ? emailAddress : recipientName,
            RecipientType = recipientType,
        });

        return this;
    }

    /// <inheritdoc/>
    public async Task SendAsync(CancellationToken cancellationToken)
    {
        // The `SecurityProtocol` property allows selecting the version of the Secure Sockets Layer (SSL) or Transport Layer Security (TLS) protocol to use for any
        // new connections; existing connections aren't changed. It is fairly true that setting the `SecurityProtocol` might be considered a bad practice; however,
        // that only applies to .NET Framework projects. Setting this property might be considered safe as long as the `SecurityProtocolType.SystemDefault` value
        // is used with an `|=` operator to ensure that any OS level security consideration is taken into account.
        // Reference: https://learn.microsoft.com/en-us/dotnet/framework/network-programming/tls
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.SystemDefault | SecurityProtocolType.Tls12;

        using var mailMessageBody = BuildMessage();
        using var mailMessage = new MimeMessage();
        mailMessage.Subject = Specification.Subject;
        mailMessage.Body = mailMessageBody;
        mailMessage.To.AddRange(Specification.To.Where(r => r.RecipientType == EmailRecipientType.TO && !string.IsNullOrWhiteSpace(r.Address)).Select(r => new MailboxAddress(r.Name, r.Address)));
        mailMessage.Cc.AddRange(Specification.To.Where(r => r.RecipientType == EmailRecipientType.CC && !string.IsNullOrWhiteSpace(r.Address)).Select(r => new MailboxAddress(r.Name, r.Address)));
        mailMessage.Bcc.AddRange(Specification.To.Where(r => r.RecipientType == EmailRecipientType.BCC && !string.IsNullOrWhiteSpace(r.Address)).Select(r => new MailboxAddress(r.Name, r.Address)));

        if (!string.IsNullOrWhiteSpace(Specification.From?.Address))
        {
            mailMessage.From.Add(new MailboxAddress(Specification.From.Name, Specification.From.Address));
        }

        using var smtpClient = new SmtpClient();

        if (SmtpClientOptions.ServerCertificateValidationCallback != null)
        {
            smtpClient.ServerCertificateValidationCallback += SmtpClientOptions.ServerCertificateValidationCallback;
        }

        await smtpClient.ConnectAsync(SmtpClientOptions.Host, SmtpClientOptions.Port, SmtpClientOptions.UseSSL, cancellationToken);
        await smtpClient.AuthenticateAsync(SmtpClientOptions.User, SmtpClientOptions.Password, cancellationToken);
        await smtpClient.SendAsync(mailMessage, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }

    /// <inheritdoc/>
    public IEmailBuilder SetBody(string body, bool isHtml = false)
    {
        Specification.Body = body;
        Specification.IsHtmlBody = isHtml;
        return this;
    }

    /// <inheritdoc/>
    public IEmailBuilder SetBody(StringBuilder body, bool isHtml = false) => SetBody(body?.ToString(), isHtml: isHtml);

    /// <inheritdoc/>
    public IEmailBuilder SetSender(string emailAddress) => SetSender(emailAddress, null);

    /// <inheritdoc/>
    public IEmailBuilder SetSender(string emailAddress, string senderName)
    {
        Guard.IsNotNullOrWhiteSpace(emailAddress);
        Guard.IsTrue(emailAddress.IsValidEmail(), nameof(emailAddress), @"Parameter is not a valid e-mail format!");

        Specification.From = new EmailSenderSpecification()
        {
            Address = emailAddress,
            Name = string.IsNullOrWhiteSpace(senderName) ? emailAddress : senderName,
        };

        return this;
    }

    /// <inheritdoc/>
    public IEmailBuilder SetDefaultSender(string senderName = null) => SetSender(SmtpClientOptions.User, senderName);

    /// <inheritdoc/>
    /// <remarks>This implementation does not allows the subject to be <see langword="null"/>.</remarks>
    public IEmailBuilder SetSubject(string subject)
    {
        Guard.IsNotNull(subject);

        Specification.Subject = subject;

        return this;
    }

    private static SmtpClientOptions ValidateOptions(SmtpClientOptions smtpClientOptions)
    {
#pragma warning disable S3236 // Caller information arguments should not be provided explicitly
        Guard.IsNotNull(smtpClientOptions);
        Guard.IsTrue(smtpClientOptions.Port > 0, nameof(smtpClientOptions.Port));
        Guard.IsNotNullOrEmpty(smtpClientOptions.Host.Trim(), nameof(smtpClientOptions.Host));
#pragma warning restore S3236 // Caller information arguments should not be provided explicitly

        return smtpClientOptions;
    }

    private MimeEntity BuildMessage()
    {
        var bodyBuilder = new BodyBuilder();

        if (Specification.IsHtmlBody)
        {
            bodyBuilder.HtmlBody = Specification.Body;
        }
        else
        {
            bodyBuilder.TextBody = Specification.Body;
        }

        foreach (var attachment in Specification.Attachments)
        {
            if (attachment.ContentType != null && MimeKit.ContentType.TryParse(attachment.ContentType.MediaType, out var contentType))
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Data, contentType);
            }
            else
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Data);
            }
        }

        return bodyBuilder.ToMessageBody();
    }
}
