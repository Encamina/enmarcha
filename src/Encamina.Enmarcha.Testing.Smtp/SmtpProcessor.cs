using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

using Encamina.Enmarcha.Email.Abstractions;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// An SMTP protocol processor.
/// </summary>
internal sealed class SmtpProcessor
{
    /* Messages */

#pragma warning disable SA1310 // Field names should not contain underscore
    private const string MESSAGE_DEFAULT_WELCOME = @"220 {0} - Welcome to a test SMTP Server.";
    private const string MESSAGE_GOODBYE = @"221 Goodbye.";
    private const string MESSAGE_AUTH_SUCCESSFUL = @"235 Authentication successful.";
    private const string MESSAGE_DEFAULT_HELO_RESPONSE = @"250 {0}";
    private const string MESSAGE_OK = @"250 OK";
    private const string MESSAGE_START_DATA = @"354 Start mail input; end with <CRLF>.<CRLF>";
    private const string MESSAGE_INVALID_ADDRESS = @"451 Address is invalid.";
    private const string MESSAGE_UNKNOWN_COMMAND = @"500 Command Unrecognized.";
    private const string MESSAGE_INVALID_ARGUMENT_COUNT = @"501 Incorrect number of arguments.";
    private const string MESSAGE_INVALID_COMMAND_ORDER = @"503 Command not allowed here.";
#pragma warning restore SA1310 // Field names should not contain underscore

    /* Regular Expressions */

    private static readonly Regex AddressRegex = new("<.+@.+>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly ILogger logger;

    private readonly ConcurrentBag<MailMessage> messageStore;

    /// <summary>The response to the <c>HELO</c> command.</summary>
    private readonly string heloResponse;

    /// <summary>The message to display to the client when they first connect.</summary>
    private readonly string welcomeMessage;

    private SmtpContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpProcessor"/> class.
    /// </summary>
    /// <param name="domain">The domain from the test SMTP server.</param>
    /// <param name="messageStore">A messages store to keep any processed SMTP message (i.e., e-mails).</param>
    /// <param name="logger">A <see cref="ILogger">logger</see> for this SMTP processor.</param>
    internal SmtpProcessor(string domain, ConcurrentBag<MailMessage> messageStore, ILogger logger)
    {
        this.logger = logger;
        this.messageStore = messageStore;

        // Initialize default messages
        welcomeMessage = string.Format(MESSAGE_DEFAULT_WELCOME, domain);
        heloResponse = string.Format(MESSAGE_DEFAULT_HELO_RESPONSE, domain);
    }

    /// <summary>
    /// Event handler for SMTP message received.
    /// </summary>
    internal event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Handles a connected TCP client and performs all necessary interaction with it to comply with <see href="https://www.rfc-editor.org/rfc/rfc821">RFC-821</see>.
    /// </summary>
    /// <remarks>This method is thread safe.</remarks>
    /// <param name="socket">The <see cref="Socket">socket</see> to list for TCP client connections.</param>
    /// <seealso href="https://www.rfc-editor.org/rfc/rfc821"/>
    internal void ProcessConnection(Socket socket)
    {
        context = new SmtpContext(socket, logger);

        SendWelcomeMessage();
        ProcessCommands();
    }

    private void HandleMailInput(string[] inputs, string currentInputLine)
    {
        if (inputs[1].StartsWith(@"from", StringComparison.OrdinalIgnoreCase))
        {
            Mail(currentInputLine[currentInputLine.IndexOf(' ')..]);
        }
        else
        {
            context.WriteLine(MESSAGE_UNKNOWN_COMMAND);
        }
    }

    private void HandleRecipientsInput(string[] inputs, string currentInputLine)
    {
        if (inputs[1].StartsWith(@"to", StringComparison.OrdinalIgnoreCase))
        {
            Rcpt(currentInputLine[currentInputLine.IndexOf(' ')..]);
        }
        else
        {
            context.WriteLine(MESSAGE_UNKNOWN_COMMAND);
        }
    }

    /// <summary>
    /// Parses a valid e-mail address out of the input string and return it. A <see langword="null"/> is returned if no address is found.
    /// </summary>
    private string ParseAddress(string input)
    {
        var match = AddressRegex.Match(input);

        if (match.Success)
        {
            var matchText = match.Value;

            // Trim off the `:<` chars
            matchText = matchText.Remove(0, 1);

            // trim off the `.` char.
            matchText = matchText.Remove(matchText.Length - 1, 1);

            return matchText;
        }

        return null;
    }

    /// <summary>
    /// Handles the commands input from the client. This message returns when the client issues the <c>QUIT</c> command.
    /// </summary>
    private void ProcessCommands()
    {
        logger.LogDebug(@"Processing Commands....");

        var isRunning = true;

        // Loop until the client quits.
        while (isRunning)
        {
            try
            {
                var inputLine = context.ReadLine();

                if (inputLine == null)
                {
                    isRunning = false;
                    context.WriteLine(MESSAGE_GOODBYE);
                    context.Close();
                    continue;
                }

                var inputs = inputLine.Split(' ');

                switch (inputs[0].ToUpperInvariant())
                {
                    case @"HELO":
                        Helo(inputs);
                        break;

                    case @"EHLO":
                        context.WriteLine(@"250-{inputs[1]}");
                        context.WriteLine(@"250 AUTH PLAIN");
                        context.LastCommand = SmtpCommands.HELO;
                        break;

                    case @"RSET":
                        Rset();
                        break;

                    case @"NOOP":
                        context.WriteLine(MESSAGE_OK);
                        break;

                    case @"QUIT":
                        isRunning = false;
                        context.WriteLine(MESSAGE_GOODBYE);
                        context.Close();
                        break;

                    case @"MAIL":
                        HandleMailInput(inputs, inputLine);
                        break;

                    case @"RCPT":
                        HandleRecipientsInput(inputs, inputLine);
                        break;

                    case @"DATA":
                        Data();
                        break;

                    case @"AUTH":
                        context.WriteLine(MESSAGE_AUTH_SUCCESSFUL);
                        break;

                    default:
                        context.WriteLine(MESSAGE_UNKNOWN_COMMAND);
                        break;
                }
            }
            catch (SocketException socketException)
            {
                if (socketException.ErrorCode == 10060)
                {
                    context.WriteLine(MESSAGE_GOODBYE);
                }
                else
                {
                    logger.LogError(@"Socket exception different than code `10060`!", socketException);
                }

                isRunning = false;
                context.Socket.Dispose();
            }
            catch (Exception exception)
            {
                logger.LogError(@"Unexpected exception processing commands!", exception);

                isRunning = false;
                context.Socket.Dispose();
            }

            logger.LogDebug(@"Done processing commands.");
        }
    }

    /// <summary>
    /// Sends the welcome greeting to the client.
    /// </summary>
    private void SendWelcomeMessage()
    {
        logger.LogDebug(@"Sending welcome message...");

        context.WriteLine(welcomeMessage);

        logger.LogDebug(@"Welcome message sent.");
    }

    /// <summary>
    /// Handles the <c>HELO</c> command.
    /// </summary>
    private void Helo(string[] inputs)
    {
        if (context.LastCommand == -1)
        {
            if (inputs.Length == 2)
            {
                context.ClientDomain = inputs[1];
                context.LastCommand = SmtpCommands.HELO;
                context.WriteLine(heloResponse);
            }
            else
            {
                context.WriteLine(MESSAGE_INVALID_ARGUMENT_COUNT);
            }
        }
        else
        {
            context.WriteLine(MESSAGE_INVALID_COMMAND_ORDER);
        }
    }

    /// <summary>
    /// Handles the <c>DATA</c> command.
    /// </summary>
    private void Data()
    {
        context.WriteLine(MESSAGE_START_DATA);

        var rawSmtpMessage = context.RawSmtpMessage;

        var clientEndPoint = (IPEndPoint)context.Socket.RemoteEndPoint;
        var header = new StringBuilder();
        header.Append(string.Format(@"Received: from ({0} [{1}])", context.ClientDomain, clientEndPoint.Address));
        header.Append(MagicStrings.EOL);
        header.Append(@"     " + DateTime.UtcNow);
        header.Append(MagicStrings.EOL);

        rawSmtpMessage.Raw.Append(header.ToString());

        ////header.Length = 0;

        var line = context.ReadLine();
        while (line is not null && !line.Equals(@"."))
        {
            rawSmtpMessage.Raw.Append(line);
            rawSmtpMessage.Raw.Append(MagicStrings.EOL);
            line = context.ReadLine();
        }

        if (messageStore is not null)
        {
            lock (messageStore)
            {
                var mailMessage = rawSmtpMessage.AsMailMessage();

                messageStore.Add(mailMessage);

                if (MessageReceived is not null)
                {
                    MessageReceived(this, new MessageReceivedEventArgs(mailMessage));
                }
            }
        }

        context.WriteLine(MESSAGE_OK);

        // Reset the connection.
        context.Reset();
    }

    /// <summary>
    /// Handle the <c>MAIL FROM:&lt;address&gt;</c> command.
    /// </summary>
    private void Mail(string argument)
    {
        var addressValid = false;

        if (context.LastCommand == SmtpCommands.HELO)
        {
            var address = ParseAddress(argument);

            if (!string.IsNullOrEmpty(address))
            {
                try
                {
                    context.LastCommand = SmtpCommands.MAIL;
                    addressValid = true;
                    context.WriteLine(MESSAGE_OK);
                }
                catch
                {
                    /* Just fall through. */
                }
            }

            // Inform the client if the address is invalid.
            if (!addressValid)
            {
                context.WriteLine(MESSAGE_INVALID_ADDRESS);
            }
        }
        else
        {
            context.WriteLine(MESSAGE_INVALID_COMMAND_ORDER);
        }
    }

    /// <summary>
    /// Handle the <c>RCPT TO:&lt;address&gt;</c> command.
    /// </summary>
    private void Rcpt(string argument)
    {
        if (context.LastCommand is SmtpCommands.MAIL or SmtpCommands.RCPT)
        {
            var address = ParseAddress(argument);

            if (!string.IsNullOrEmpty(address))
            {
                try
                {
                    var emailAddress = new EmailRecipientSpecification()
                    {
                        Address = address,
                    };

                    context.RawSmtpMessage.AddRecipient(emailAddress);
                    context.LastCommand = SmtpCommands.RCPT;
                    context.WriteLine(MESSAGE_OK);
                }
                catch
                {
                    context.WriteLine(MESSAGE_INVALID_ADDRESS);
                }
            }
            else
            {
                context.WriteLine(MESSAGE_INVALID_ADDRESS);
            }
        }
        else
        {
            context.WriteLine(MESSAGE_INVALID_COMMAND_ORDER);
        }
    }

    /// <summary>
    /// Reset the connection state.
    /// </summary>
    private void Rset()
    {
        if (context.LastCommand != -1)
        {
            // Dump the message and reset the context.
            context.Reset();
            context.WriteLine(MESSAGE_OK);
        }
        else
        {
            context.WriteLine(MESSAGE_INVALID_COMMAND_ORDER);
        }
    }
}
