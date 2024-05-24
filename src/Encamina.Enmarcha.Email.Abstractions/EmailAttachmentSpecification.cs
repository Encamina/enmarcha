using System.Net.Mime;

namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents an e-mail attachment specification.
/// </summary>
public class EmailAttachmentSpecification
{
    /// <summary>
    /// Gets the file name of the attachment in this specification.
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// Gets the <see cref="ContentType">content type</see> of the attachment in this specification.
    /// </summary>
    public ContentType? ContentType { get; init; }

    /// <summary>
    /// Gets the binary data of the attachment in this specification.
    /// </summary>
    public byte[] Data { get; init; }
}
