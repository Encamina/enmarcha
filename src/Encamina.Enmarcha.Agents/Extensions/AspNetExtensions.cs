using System.Collections.Concurrent;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Options;

using Microsoft.Agents.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Validators;

namespace Encamina.Enmarcha.Agents.Extensions;

/// <summary>
/// Extension methods for ASP.NET related to authentication to agents.
/// Code based on https://github.com/microsoft/Agents/blob/e771ea9cdbbf859f5e8ca4931257fb000973dadd/samples/dotnet/quickstart/AspNetExtensions.cs.
/// </summary>
public static class AspNetExtensions
{
    private static readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> OpenIdMetadataCache = new();

    /// <summary>
    /// Adds AspNet token validation typical for ABS/SMBA and agent-to-agent using settings in configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to use.</param>
    /// <param name="configuration">The configuration to read settings from.</param>
    /// <param name="tokenValidationSectionName">Name of the config section to read.</param>
    /// <remarks>
    /// <para>This extension reads <see cref="TokenValidationOptions"/> settings from configuration. If configuration is missing JWT token
    /// is not enabled.</para>
    /// <p>The minimum, but typical, configuration is:</p>
    /// <code>
    /// "TokenValidation": {
    ///    "Enabled": boolean,
    ///    "Audiences": [
    ///      "{{ClientId}}" // this is the Client ID used for the Azure Bot
    ///    ],
    ///    "TenantId": "{{TenantId}}"
    /// }
    /// </code>
    /// <para>The full options are:</para>
    /// <code>
    /// "TokenValidation": {
    ///   "Enabled": boolean,
    ///   "Audiences": [
    ///     "{required:agent-appid}"
    ///   ],
    ///   "TenantId": "{recommended:tenant-id}",
    ///   "ValidIssuers": [
    ///     "{default:Public-AzureBotService}"
    ///   ],
    ///   "IsGov": {optional:false},
    ///   "AzureBotServiceOpenIdMetadataUrl": optional,
    ///   "OpenIdMetadataUrl": optional,
    ///   "AzureBotServiceTokenHandling": "{optional:true}"
    ///   "OpenIdMetadataRefresh": "optional-12:00:00"
    /// }
    /// </code>
    /// </remarks>
    public static void AddAgentAspNetAuthentication(this IServiceCollection services, IConfiguration configuration, string tokenValidationSectionName = "TokenValidation")
    {
        var tokenValidationSection = configuration.GetSection(tokenValidationSectionName);

        if (!tokenValidationSection.Exists() || !tokenValidationSection.GetValue("Enabled", true))
        {
            // Noop if TokenValidation section missing or disabled.
            System.Diagnostics.Trace.WriteLine("AddAgentAspNetAuthentication: Auth disabled");
            return;
        }

        services.AddAgentAspNetAuthentication(tokenValidationSection.Get<TokenValidationOptions>()!);
    }

    /// <summary>
    /// Adds AspNet token validation typical for ABS/SMBA and agent-to-agent.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to use.</param>
    /// <param name="validationOptions">The <see cref="TokenValidationOptions"/> to use.</param>
    /// <exception cref="ArgumentException">Thrown if validationOptions is invalid.</exception>
    public static void AddAgentAspNetAuthentication(this IServiceCollection services, TokenValidationOptions validationOptions)
    {
        Guard.IsNotNull(validationOptions);

        // Must have at least one Audience.
        if (validationOptions.Audiences == null || validationOptions.Audiences.Count == 0)
        {
            throw new ArgumentException($"{nameof(TokenValidationOptions)}:Audiences requires at least one ClientId");
        }

        // Audience values must be GUID's
        if (validationOptions.Audiences.Any(audience => !Guid.TryParse(audience, out _)))
        {
            throw new ArgumentException($"{nameof(TokenValidationOptions)}:Audiences values must be a GUID");
        }

        // If ValidIssuers is empty, default for ABS Public Cloud
        if (validationOptions.ValidIssuers == null || validationOptions.ValidIssuers.Count == 0)
        {
            validationOptions.ValidIssuers =
            [
                "https://api.botframework.com",
                "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0",
                "https://sts.windows.net/69e9b82d-4842-4902-8d1e-abc5b98a55e8/",
                "https://login.microsoftonline.com/69e9b82d-4842-4902-8d1e-abc5b98a55e8/v2.0",
            ];

            if (!string.IsNullOrEmpty(validationOptions.TenantId) && Guid.TryParse(validationOptions.TenantId, out _))
            {
                validationOptions.ValidIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV1, validationOptions.TenantId));
                validationOptions.ValidIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV2, validationOptions.TenantId));
            }
        }

        // If the `AzureBotServiceOpenIdMetadataUrl` setting is not specified, use the default based on `IsGov`. This is what is used to authenticate ABS tokens.
        if (string.IsNullOrEmpty(validationOptions.AzureBotServiceOpenIdMetadataUrl))
        {
            validationOptions.AzureBotServiceOpenIdMetadataUrl = validationOptions.IsGov ? AuthenticationConstants.GovAzureBotServiceOpenIdMetadataUrl : AuthenticationConstants.PublicAzureBotServiceOpenIdMetadataUrl;
        }

        // If the `OpenIdMetadataUrl` setting is not specified, use the default based on `IsGov`. This is what is used to authenticate Entra ID tokens.
        if (string.IsNullOrEmpty(validationOptions.OpenIdMetadataUrl))
        {
            validationOptions.OpenIdMetadataUrl = validationOptions.IsGov ? AuthenticationConstants.GovOpenIdMetadataUrl : AuthenticationConstants.PublicOpenIdMetadataUrl;
        }

        var openIdMetadataRefresh = validationOptions.OpenIdMetadataRefresh ?? BaseConfigurationManager.DefaultAutomaticRefreshInterval;

        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidIssuers = validationOptions.ValidIssuers,
                ValidAudiences = validationOptions.Audiences,
                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
            };

            // Using Microsoft.IdentityModel.Validators
            options.TokenValidationParameters.EnableAadSigningKeyIssuerValidation();

            options.Events = new JwtBearerEvents
            {
                // Create a ConfigurationManager based on the requestor.  This is to handle ABS non-Entra tokens.
                OnMessageReceived = async context =>
                {
                    var authorizationHeader = context.Request.Headers.Authorization.ToString();

                    if (string.IsNullOrEmpty(authorizationHeader))
                    {
                        // Default to AadTokenValidation handling
                        context.Options.TokenValidationParameters.ConfigurationManager ??= options.ConfigurationManager as BaseConfigurationManager;
                        await Task.CompletedTask.ConfigureAwait(false);
                        return;
                    }

                    var parts = authorizationHeader.Split(' ');
                    if (parts is not ["Bearer", _])
                    {
                        // Default to AadTokenValidation handling
                        context.Options.TokenValidationParameters.ConfigurationManager ??= options.ConfigurationManager as BaseConfigurationManager;
                        await Task.CompletedTask.ConfigureAwait(false);
                        return;
                    }

                    var token = new JwtSecurityToken(parts[1]);
                    var issuer = token.Claims.FirstOrDefault(claim => claim.Type == AuthenticationConstants.IssuerClaim)?.Value!;

                    if (validationOptions.AzureBotServiceTokenHandling && AuthenticationConstants.BotFrameworkTokenIssuer.Equals(issuer))
                    {
                        // Use the Azure Bot authority for this configuration manager
                        context.Options.TokenValidationParameters.ConfigurationManager = OpenIdMetadataCache.GetOrAdd(validationOptions.AzureBotServiceOpenIdMetadataUrl, _ => new ConfigurationManager<OpenIdConnectConfiguration>(validationOptions.AzureBotServiceOpenIdMetadataUrl, new OpenIdConnectConfigurationRetriever(), new HttpClient())
                        {
                            AutomaticRefreshInterval = openIdMetadataRefresh,
                        });
                    }
                    else
                    {
                        context.Options.TokenValidationParameters.ConfigurationManager = OpenIdMetadataCache.GetOrAdd(validationOptions.OpenIdMetadataUrl, _ => new ConfigurationManager<OpenIdConnectConfiguration>(validationOptions.OpenIdMetadataUrl, new OpenIdConnectConfigurationRetriever(), new HttpClient())
                        {
                            AutomaticRefreshInterval = openIdMetadataRefresh,
                        });
                    }

                    await Task.CompletedTask.ConfigureAwait(false);
                },

                OnTokenValidated = _ => Task.CompletedTask,
                OnForbidden = _ => Task.CompletedTask,
                OnAuthenticationFailed = _ => Task.CompletedTask,
            };
        });
    }
}
