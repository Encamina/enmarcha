using Encamina.Enmarcha.AspNet.Mvc.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure authentication mechanisms.
/// </summary>
public static class AuthenticationBuilderExtensions
{
    /// <summary>
    /// Adds OpenId Connect authentication using the default values.
    /// </summary>
    /// <remarks>
    /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
    /// to request and receive information about authenticated sessions and end-users.
    /// </remarks>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed so that additional calls can be chained.</returns>
    public static AuthenticationBuilder AddOpenIdConnectAuthentication(this AuthenticationBuilder builder) => builder.AddOpenIdConnectAuthentication(_ => { });

    /// <summary>
    /// Adds OpenId Connect authentication using Azure Active Directory configuration options.
    /// </summary>
    /// <remarks>
    /// OpenID Connect is an identity layer on top of the OAuth 2.0 protocol. It allows clients
    /// to request and receive information about authenticated sessions and end-users.
    /// </remarks>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">Configuration options with values from Azure Active Directory.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed so that additional calls can be chained.</returns>
    public static AuthenticationBuilder AddOpenIdConnectAuthentication(this AuthenticationBuilder builder, Action<AzureActiveDirectoryOptions> configureOptions)
    {
        builder.AddOpenIdConnect();
        builder.Services.Configure(configureOptions)
                        .TryAddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureOpenIdAzureOptions>();

        return builder;
    }

    /// <summary>
    /// Adds JWT-bearer authentication using the default scheme <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
    /// </summary>
    /// <remarks>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </remarks>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed so that additional calls can be chained.</returns>
    public static AuthenticationBuilder AddJwtBearerAuthentication(this AuthenticationBuilder builder) => builder.AddJwtBearerAuthentication(_ => { });

    /// <summary>
    /// Adds JWT-bearer authentication using Azure Active Directory configuration options.
    /// </summary>
    /// <remarks>
    /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
    /// </remarks>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">Configuration options with values from Azure Active Directory.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed so that additional calls can be chained.</returns>
    public static AuthenticationBuilder AddJwtBearerAuthentication(this AuthenticationBuilder builder, Action<AzureActiveDirectoryOptions> configureOptions)
    {
        builder.AddJwtBearer();
        builder.Services.Configure(configureOptions)
                        .TryAddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerAzureOptions>();

        return builder;
    }

    private abstract class ConfigureAzureOptionsBase<T> : IConfigureNamedOptions<T> where T : AuthenticationSchemeOptions
    {
        protected ConfigureAzureOptionsBase(IOptions<AzureActiveDirectoryOptions> azureOptions)
        {
            AzureActiveDirectoryOptions = azureOptions.Value;
        }

        protected AzureActiveDirectoryOptions AzureActiveDirectoryOptions { get; }

        public abstract void Configure(string name, T options);

        public void Configure(T options) => Configure(Options.Options.DefaultName, options);
    }

    private sealed class ConfigureJwtBearerAzureOptions : ConfigureAzureOptionsBase<JwtBearerOptions>
    {
        public ConfigureJwtBearerAzureOptions(IOptions<AzureActiveDirectoryOptions> azureOptions) : base(azureOptions)
        {
        }

        public override void Configure(string name, JwtBearerOptions options)
        {
            options.Audience = AzureActiveDirectoryOptions.ClientId;
            options.Authority = $"{AzureActiveDirectoryOptions.Instance}{AzureActiveDirectoryOptions.TenantId}";
        }
    }

    private sealed class ConfigureOpenIdAzureOptions : ConfigureAzureOptionsBase<OpenIdConnectOptions>
    {
        public ConfigureOpenIdAzureOptions(IOptions<AzureActiveDirectoryOptions> azureOptions) : base(azureOptions)
        {
        }

        public override void Configure(string name, OpenIdConnectOptions options)
        {
            options.ClientId = AzureActiveDirectoryOptions.ClientId;
            options.Authority = $"{AzureActiveDirectoryOptions.Instance}{AzureActiveDirectoryOptions.TenantId}";
            options.UseTokenLifetime = true;
            options.CallbackPath = AzureActiveDirectoryOptions.CallbackPath;
            options.RequireHttpsMetadata = false;
        }
    }
}
