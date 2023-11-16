namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents an e-mails recipient's (address) specification.
/// </summary>
public class EmailRecipientSpecification : EmailAddressSpecification
{
    /// <summary>
    /// Gets the e-mails recipient type. Defaults to <see cref="EmailRecipientType.TO"/>.
    /// </summary>
    public EmailRecipientType RecipientType { get; init; } = EmailRecipientType.TO;

    /// <inheritdoc />
    public override string ToString() => $@"{RecipientType}: {base.ToString()}";
}
