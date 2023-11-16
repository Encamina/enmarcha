using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Encamina.Enmarcha.Testing.Smtp;

/// <summary>
/// Configuration for a test SMPT server.
/// </summary>
public sealed class Configuration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class by default.
    /// </summary>
    /// <remarks>
    /// This constructor is <c><see langword="private"/></c> to prevent a default instance of the <see cref="Configuration"/> class from being created.
    /// </remarks>
    private Configuration()
    {
        IPAddress = IPAddress.Any;
        ReuseAddress = true;
    }

    /// <summary>
    /// Gets the domain that identifies the test SMTP server.
    /// </summary>
    public string Domain { get; private set; }

    /// <summary>
    /// Gets the port configured for the test SMTP server.
    /// </summary>
    public int Port { get; private set; }

    /// <summary>
    /// Gets the IP address configured for the test SMTP server.
    /// </summary>
    public IPAddress IPAddress { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the IP address for the test SMTP server should be reused or not.
    /// </summary>
    public bool ReuseAddress { get; private set; }

    /// <summary>
    /// Gets the logger of the test SMTP server.
    /// </summary>
    internal ILogger Logger { get; private set; }

    /// <summary>
    /// Gets and instance of a configuration for a test SMTP server.
    /// </summary>
    /// <returns>A new instance of a configuration for a test SMTP server.</returns>
    public static Configuration Configure()
    {
        return new Configuration();
    }

    /// <summary>
    /// Creates a valid instance of a <see cref="SmtpServer">test SMTP server</see> using the current configuration.
    /// </summary>
    /// <returns>
    /// A valid instance of a <see cref="SmtpServer">test SMTP server</see> configured with the current configuration.
    /// </returns>
    public SmtpServer Build()
    {
        if (string.IsNullOrWhiteSpace(Domain))
        {
            Domain = @"localhost";
        }

        if (Port <= 0)
        {
            WithRandomPort();
        }

        Logger ??= NullLogger.Instance;

        return SmtpServer.Start(this);
    }

    /// <summary>
    /// Indicates that the domain that identifies the test SMTP server.
    /// </summary>
    /// <param name="domain">A domain to identify the test SMTP server.</param>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration WithDomain(string domain)
    {
        Domain = domain;

        return this;
    }

    /// <summary>
    /// Indicates that the IP address for the test SMTP server <b>should not</b> be reused.
    /// </summary>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration DoNotReuseAddress()
    {
        ReuseAddress = false;
        return this;
    }

    /// <summary>
    /// Indicates that a random number should be use as port number for the test SMTP server.
    /// </summary>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration WithRandomPort()
    {
        Port = GetRandomUnusedPort();
        return this;
    }

    /// <summary>
    /// Sets the specific port number for the test SMTP server.
    /// </summary>
    /// <param name="port">A specific port number for the test SMTP server.</param>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration WithPort(int port)
    {
        Port = port;

        return this;
    }

    /// <summary>
    /// Sets the IP address to use with the test SMTP server.
    /// </summary>
    /// <param name="address">An IP address to use with the test SMTP server.</param>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration WithIPAddress(IPAddress address)
    {
        IPAddress = address;

        return this;
    }

    /// <summary>
    /// Sets the <see cref="ILogger">logger</see> to use with the test SMTP server.
    /// </summary>
    /// <param name="logger">A <see cref="ILogger">logger</see> to use with the test SMTP server.</param>
    /// <returns>This current <see cref="Configuration">configuration</see> so that additional calls can be chained.</returns>
    public Configuration WithLogger(ILogger logger)
    {
        Logger = logger;

        return this;
    }

    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);

        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;

        listener.Stop();

        return port;
    }
}
