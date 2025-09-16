using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Logging;
using Encamina.Enmarcha.Agents.Options;

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Agents.Extensions;

/// <summary>
/// Extension methods for <see cref="ILoggingBuilder"/>.
/// </summary>
public static class ILoggingBuilderExtensions
{
    /// <summary>
    /// Adds an ApplicationInsights logger named <see cref="ApplicationInsightsConversationScopedLoggerProvider"/> to the factory.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configureTelemetryConfiguration">Action to configure telemetry configuration.</param>
    /// <param name="configureApplicationInsightsLoggerOptions">Action to configure ApplicationInsights logger.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> passed as argument.</returns>
    public static ILoggingBuilder AddApplicationInsightsConversationScoped(this ILoggingBuilder builder, Action<TelemetryConfiguration> configureTelemetryConfiguration, Action<ApplicationInsightsConversationScopedLoggerOptions> configureApplicationInsightsLoggerOptions)
    {
        Guard.IsNotNull(builder);
        Guard.IsNotNull(configureTelemetryConfiguration);
        Guard.IsNotNull(configureApplicationInsightsLoggerOptions);

        builder.Services.Configure(configureTelemetryConfiguration);
        builder.Services.AddSingleton<ILoggerProvider, ApplicationInsightsConversationScopedLoggerProvider>();
        builder.Services.Configure(configureApplicationInsightsLoggerOptions);
        return builder;
    }
}
