using System.Reflection;

using Encamina.Enmarcha.Agents.Options;
using Encamina.Enmarcha.Core;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Agents.Logging;

/// <summary>
/// <see cref="ILoggerProvider"/> implementation that creates returns instances of <see cref="ApplicationInsightsConversationScopedLoggerProvider"/>.
/// </summary>
public class ApplicationInsightsConversationScopedLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    /// <summary>
    /// The application insights logger options.
    /// </summary>
    private readonly ApplicationInsightsConversationScopedLoggerOptions applicationInsightsLoggerOptions;

    /// <summary>
    /// The telemetry client to be used to log messages to Application Insights.
    /// </summary>
    private readonly TelemetryClient telemetryClient;

    /// <summary>
    /// The http context accessor to extract the ConversationId from.
    /// </summary>
    private readonly IHttpContextAccessor httpContextAccessor;

    /// <summary>
    /// The external scope provider to allow setting scope data in messages.
    /// </summary>
    private IExternalScopeProvider externalScopeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationInsightsConversationScopedLoggerProvider"/> class.
    /// </summary>
    /// <param name="telemetryConfigurationOptions">The telemetry configuration options..</param>
    /// <param name="applicationInsightsLoggerOptions">The application insights logger options.</param>
    /// <param name="httpContextAccessor">The http context accessor to extract the ConversationId from.</param>
    public ApplicationInsightsConversationScopedLoggerProvider(IOptions<TelemetryConfiguration> telemetryConfigurationOptions, IOptions<ApplicationInsightsConversationScopedLoggerOptions> applicationInsightsLoggerOptions, IHttpContextAccessor httpContextAccessor)
    {
        this.applicationInsightsLoggerOptions = applicationInsightsLoggerOptions?.Value ?? throw new ArgumentNullException(nameof(applicationInsightsLoggerOptions));
        this.httpContextAccessor = httpContextAccessor;

        telemetryClient = new TelemetryClient(telemetryConfigurationOptions.Value);
        telemetryClient.Context.GetInternalContext().SdkVersion = SdkVersionUtils.GetSdkVersion("il:", Assembly.GetAssembly(typeof(ApplicationInsightsConversationScopedLoggerProvider))!);
    }

    /// <summary>
    /// Creates a new <see cref="ILogger" /> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>An <see cref="ILogger"/> instance to be used for logging.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new ApplicationInsightsConversationScopedLogger(categoryName, telemetryClient, applicationInsightsLoggerOptions, httpContextAccessor)
        {
            ExternalScopeProvider = externalScopeProvider,
        };
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Sets the scope provider. This method also updates all the existing logger to also use the new scope provider.
    /// </summary>
    /// <param name="scopeProvider">The external scope provider.</param>
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        externalScopeProvider = scopeProvider;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="releasedManagedResources">Release managed resources.</param>
    protected virtual void Dispose(bool releasedManagedResources)
    {
        if (releasedManagedResources && applicationInsightsLoggerOptions.FlushOnDispose)
        {
            telemetryClient.Flush();
        }
    }
}
