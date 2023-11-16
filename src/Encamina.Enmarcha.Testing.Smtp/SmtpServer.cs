using System.Collections.Concurrent;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// An SMTP server for testing purposes.
/// </summary>
public sealed class SmtpServer : IDisposable
{
    private readonly object lockObj = new();

    private readonly CancellationTokenSource cancellation = new();

    private readonly ConcurrentBag<MailMessage> messageStore = new();

    private TcpListener tcpListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpServer"/> class on a specific port number.
    /// </summary>
    /// <param name="port">The port for this instance of an SMTP server.</param>
    public SmtpServer(int port) : this(port, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpServer"/> class on a specific port number and using a given <see cref="ILogger">logger</see>.
    /// </summary>
    /// <param name="port">The port for this instance of a test SMTP server.</param>
    /// <param name="logger">A <see cref="ILogger">logger</see> to use with this test SMTP server.</param>
    public SmtpServer(int port, ILogger logger) : this(Configuration.Configure().WithPort(port).WithLogger(logger))
    {
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="SmtpServer"/> class from being created.
    /// </summary>
    /// <param name="configuration">The configuration for this instance of a test SMTP server.</param>
    private SmtpServer(Configuration configuration)
    {
        Configuration = configuration;
        ServerReady = new AutoResetEvent(false);
    }

    /// <summary>
    /// Event handler for SMTP message received.
    /// </summary>
    internal event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Gets the current <see cref="Configuration">configuration</see> of this test SMTP server.
    /// </summary>
    public Configuration Configuration { get; init; }

    /// <summary>
    /// Gets the current count of received messages.
    /// </summary>
    public int ReceivedMessagesCount
    {
        get
        {
            lock (lockObj)
            {
                return messageStore.Count;
            }
        }
    }

    /// <summary>
    /// Gets the current collection of received messages.
    /// </summary>
    public IReadOnlyList<MailMessage> ReceivedMessages
    {
        get
        {
            lock (lockObj)
            {
                return messageStore.ToArray();
            }
        }
    }

    private AutoResetEvent ServerReady { get; init; }

    /// <summary>
    /// Starts a test SMTP server using a random port number.
    /// </summary>
    /// <returns>A valid instance of a <see cref="SmtpServer">test SMTP server</see>.</returns>
    public static SmtpServer Start()
    {
        return Start(Configuration.Configure().WithRandomPort());
    }

    /// <summary>
    /// Starts a test SMTP server using the given <see cref="Configuration">configuration</see>.
    /// </summary>
    /// <param name="configuration">The <see cref="Configuration">configuration</see> to use when starting this test SMTP server.</param>
    /// <returns>A valid instance of a <see cref="SmtpServer">test SMTP server</see>. using the given <paramref name="configuration"/>.</returns>
    public static SmtpServer Start(Configuration configuration)
    {
        var server = new SmtpServer(configuration);
        server.StartListening();
        server.ServerReady.WaitOne();
        return server;
    }

    /// <summary>
    /// Stops this test SMTP server.
    /// </summary>
    public void Stop()
    {
        Configuration.Logger.LogDebug(@"Stoping server...");

        try
        {
            lock (lockObj)
            {
                cancellation.Cancel();

                // Kick the server accept loop
                if (tcpListener != null)
                {
                    Configuration.Logger.LogDebug(@"Stoping inner TCP listener...");
                    tcpListener.Stop();
                    Configuration.Logger.LogDebug(@"Inner TCP listener stopped.");
                }

                tcpListener = null;
            }
        }
        catch (Exception exception)
        {
            Configuration.Logger.LogError(@"Unexpected exception stopping server!", exception);
        }
        finally
        {
            Configuration.Logger.LogDebug(@"Server Stopped.");
            ServerReady.Close();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Stop();
    }

    private void StartListening()
    {
        Configuration.Logger.LogInformation("Starting server...");

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Configuration.Port < 1000)
        {
            Configuration.Logger.LogWarning($@"POSIX system detected. Administrative access may be needed to open port: {Configuration.Port}.");
        }

        tcpListener = new TcpListener(new IPEndPoint(Configuration.IPAddress, Configuration.Port));

        // This will prevent throwing an exception if the server is stopped, and then restarted again with the same port.
        if (Configuration.ReuseAddress)
        {
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        }

        tcpListener.Start();

        Configuration.Logger.LogDebug($@"Started inner TCP listener at port {Configuration.Port}.");

        ServerReady.Set();

        try
        {
            Task.Factory.StartNew(async () =>
            {
                while (tcpListener.Server.IsBound)
                {
                    var socket = await tcpListener.AcceptSocketAsync();

                    if (socket != null)
                    {
                        SocketHandler(socket);
                    }
                }
            }, cancellation.Token);
        }
        catch (Exception exception)
        {
            Configuration.Logger.LogError(@"Unexpected exception starting the server!", exception);
        }
    }

    private void SocketHandler(Socket socket)
    {
        if (cancellation.IsCancellationRequested)
        {
            return;
        }

        Configuration.Logger.LogDebug(@"Entering socket handler...");

        try
        {
            using (socket)
            {
                Configuration.Logger.LogDebug(@"Socket accepted and ready to be processed.");

                var processor = new SmtpProcessor(Configuration.Domain, messageStore, Configuration.Logger);
                processor.MessageReceived += (_, args) => MessageReceived?.Invoke(this, args);
                processor.ProcessConnection(socket);
            }
        }
        catch (ObjectDisposedException objectDisposedException)
        {
            Configuration.Logger.LogWarning(@"Object disposed exception catcher. This sould be expected ONLY if the server was stopped!", objectDisposedException);
        }
        catch (SocketException socketException)
        {
            Configuration.Logger.LogError(@"Socket exception!", socketException);
        }
    }
}
