namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Type of email recipient.
/// </summary>
public enum EmailRecipientType
{
    /// <summary>
    /// Main recipient.
    /// </summary>
    TO,

    /// <summary>
    /// Carbon copy (CC) recipient.
    /// </summary>
    CC,

    /// <summary>
    /// Blind carbon copy (BCC) recipient.
    /// </summary>
    BCC,
}
