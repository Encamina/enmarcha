namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// Values of SMTP commands that are supported by this implementation.
/// </summary>
internal static class SmtpCommands
{
    /// <summary>
    /// Identifies the domain name of the sending host to SMTP.
    /// </summary>
    /// <remarks>
    /// Use the <c>HELO</c> command to identify the domain name of the sending host to SMTP before you issue a <c>MAIL FROM</c> command.
    /// </remarks>
    internal const int HELO = 0;

    /// <summary>
    /// Resets the SMTP processing to the initial state.
    /// </summary>
    /// <remarks>
    /// Use the <c>RSET</c> command to reset SMTP processing to the initial state.
    /// The sender and recipient buffers are erased and the process is ready to begin a new mail transaction.
    /// </remarks>
    internal const int RSET = 1;

    /// <summary>
    /// Returns a <c>250 OK</c> code when SMTP is responding.
    /// </summary>
    internal const int NOOP = 2;

    /// <summary>
    /// Stops SMTP processing.
    /// </summary>
    /// <remarks>Ensure that this the last command in the spool file.</remarks>
    internal const int QUIT = 3;

    /// <summary>
    /// Specifies a mail sender. Also known as <c>MAIL FROM</c>.
    /// </summary>
    /// <remarks>Use the <c>MAIL FROM</c> command after a <c>HELO</c> or <c>EHLO</c> command.</remarks>
    internal const int MAIL = 4;

    /// <summary>
    /// Specifies the mail recipients. Also known as <c>RCPT TO</c>.
    /// </summary>
    /// <remarks>Use the <c>RCPT TO</c> command to specify mail recipients.</remarks>
    internal const int RCPT = 5;

    /// <summary>
    /// Defines information as the data text of the mail body.
    /// </summary>
    /// <remarks>
    /// Use the <c>DATA</c> command after a <c>HELO</c> or <c>EHLO</c> command, a <c>MAIL FROM</c> command, and at least one <c>RCPT TO</c> command have been accepted.
    /// </remarks>
    internal const int DATA = 6;
}
