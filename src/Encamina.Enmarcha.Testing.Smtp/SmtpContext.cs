using System.Net.Sockets;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// Maintains the current state for any SMTP client connection.
/// </summary>
/// <remarks>
/// This class is similar to a HTTP Session.  It is used to maintain all the state information about the current connection.
/// </remarks>
internal sealed class SmtpContext
{
    private readonly Encoding encoding;

    private readonly ILogger logger;

    /// <summary>
    /// It is possible that more than one line will be in the queue at any one time, so a store is needed for any input
    /// that has been read from the socket but has not been requested by the «ReadLine» command yet.
    /// </summary>
    private StringBuilder inputBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpContext"/> class.
    /// </summary>
    /// <param name="socket">The socket for this SMTP context.</param>
    /// <param name="logger">A <see cref="ILogger">logger</see> for this SMTP context.</param>
    internal SmtpContext(Socket socket, ILogger logger)
    {
        this.logger = logger;
        ClientDomain = string.Empty;
        LastCommand = -1;
        Socket = socket;

        // Set the encoding to send and receive data from the socket as ASCII,
        // which (perhaps sadly...) is the most commont encoding for e-mails.
        encoding = Encoding.ASCII;

        // Initialize the input buffer.
        inputBuffer = new StringBuilder();

        RawSmtpMessage = new RawSmtpMessage();
    }

    /// <summary>
    /// Gets or sets the client domain, as specified by the <c>HELO</c> command.
    /// </summary>
    internal string ClientDomain { get; set; }

    /// <summary>
    /// Gets or sets the last successful command received.
    /// </summary>
    internal int LastCommand { get; set; }

    /// <summary>
    /// Gets the raw SMTP message that is currently being received.
    /// </summary>
    internal RawSmtpMessage RawSmtpMessage { get; private set; }

    /// <summary>
    /// Gets the socket that is connected to the client.
    /// </summary>
    internal Socket Socket { get; init; }

    /// <summary>
    /// Closes the socket connection to the client and performs any clean-up.
    /// </summary>
    internal void Close()
    {
        logger.LogDebug(@"Closing the SMTP context...");

        inputBuffer.Length = 0;

        Socket.Close();

        logger.LogDebug(@"SMTP context closed.");
    }

    /// <summary>
    /// Reads an entire line from the socket.
    /// </summary>
    /// <returns>The line read from the socket.</returns>
    /// <remarks>This method will block until an entire line has been read.</remarks>
    internal string ReadLine()
    {
        // If we already buffered another line, just return
        // from the buffer.
        var output = ReadBuffer();

        if (output != null)
        {
            return output;
        }

        // Otherwise, read more input.
        var byteBuffer = new byte[80];
        int count;

        // Read from the socket until an entire line has been read.
        do
        {
            // Read the input data.
            count = Socket.Receive(byteBuffer);

            if (count == 0)
            {
                return null;
            }

            inputBuffer.Append(encoding.GetString(byteBuffer, 0, count));
        }
        while ((output = ReadBuffer()) == null);

        return output;
    }

    /// <summary>
    /// Resets this context for a new message.
    /// </summary>
    internal void Reset()
    {
        logger.LogDebug(@"Resetting the SMTP context...");

        inputBuffer.Length = 0;

        RawSmtpMessage = new RawSmtpMessage();

        LastCommand = SmtpCommands.HELO;

        logger.LogDebug(@"Done resetting the SMTP context.");
    }

    /// <summary>
    /// Writes a data string to the socket as an entire line.
    /// </summary>
    /// <remarks>
    /// This method will append the end of line characters ('<c>\r\n</c>'), so the data parameter should not contain them.
    /// </remarks>
    /// <param name="data">The data string to write to the client.</param>
    internal void WriteLine(string data)
    {
        Socket.Send(encoding.GetBytes(data + MagicStrings.EOL));
    }

    /// <summary>
    /// Get the first full line in the input buffer, or <see langword="null"/> if there is no line in the buffer.
    /// If a line is found, it will be removed from the buffer.
    /// </summary>
    private string ReadBuffer()
    {
        // If the buffer has data, check for a full line.
        if (inputBuffer.Length > 0)
        {
            var buffer = inputBuffer.ToString();
            var eolIndex = buffer.IndexOf(MagicStrings.EOL, StringComparison.OrdinalIgnoreCase);

            if (eolIndex != -1)
            {
                var output = buffer[..eolIndex];

                inputBuffer = new StringBuilder(buffer[(eolIndex + 2)..]);

                return output;
            }
        }

        return null;
    }
}
