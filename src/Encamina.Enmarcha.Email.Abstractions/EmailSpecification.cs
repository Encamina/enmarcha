namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// An agnostic representation of an e-mail specification.
/// </summary>
public class EmailSpecification
{
    /// <summary>
    /// Gets or sets the e-mail address of the sender of an e-mail.
    /// </summary>
    public EmailSenderSpecification From { get; set; }

    /// <summary>
    /// Gets or sets the e-mail subject.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// Gets the collection of e-mail addresses of the recipients of an e-mail.
    /// </summary>
    public ICollection<EmailRecipientSpecification> To { get; } = new List<EmailRecipientSpecification>();

    /// <summary>
    /// Gets the collection of e-mail attachments for an e-mail.
    /// </summary>
    public ICollection<EmailAttachmentSpecification> Attachments { get; } = new List<EmailAttachmentSpecification>();

    /// <summary>
    /// Gets or sets the e-mail body.
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Body">body</see> of the e-mail is an HTML or not.
    /// </summary>
    public bool IsHtmlBody { get; set; }
}
