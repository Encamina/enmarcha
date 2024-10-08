﻿using System.ComponentModel.DataAnnotations;
using System.Net.Security;

using Encamina.Enmarcha.Core.DataAnnotations;
using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.Email.Abstractions;

/// <summary>
/// Represents common SMTP configuration parameters or options.
/// </summary>
public class SmtpClientOptions : INameable
{
    private string? name = null;

    /// <summary>
    /// Gets a value indicating whether authentication is required to connect to the SMTP service.
    /// </summary>
    /// <remarks>
    /// The default value is <see langword="true"/>. Set this to <see langword="false"/> if the SMTP service does not require authentication.
    /// </remarks>
    public bool AuthenticationRequired { get; init; } = true;

    /// <summary>
    /// Gets or sets the host name of the SMTP service.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string Host { get; set; }

    /// <inheritdoc/>
    /// <remarks><b>IMPORTANT</b>: The name should be unique.</remarks>
    public string Name
    {
        get { return string.IsNullOrWhiteSpace(name) ? $@"{Host}:{Port}" : name; }
        set => name = value;
    }

    /// <summary>
    /// Gets or sets the password credential required to connect to the SMTP service.
    /// <para>This property is only required if <see cref="AuthenticationRequired"/> is set to <see langword="false"/>.</para>
    /// </summary>
    [RequiredIf(nameof(AuthenticationRequired), conditionalValue: true, allowEmpty: false)]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the port for the SMTP service. Default value is <c>587</c>.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    /// <summary>
    /// Gets or sets a callback function to validate the (mail) server certificate.
    /// </summary>
    /// <remarks>
    /// This property should be set before any connection attempt.
    /// </remarks>
    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

    /// <summary>
    /// Gets or sets the user credential required to connect to the SMTP service.
    /// <para>This property is only required if <see cref="AuthenticationRequired"/> is set to <see langword="false"/>.</para>
    /// </summary>
    [RequiredIf(nameof(AuthenticationRequired), conditionalValue: true, allowEmpty: false)]
    public string? User { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether SSL should be use. Default is <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// When setting this property as <see langword="true"/>, check the value of <seealso cref="Port"/>.
    /// </remarks>
    public bool UseSSL { get; set; } = false;
}
