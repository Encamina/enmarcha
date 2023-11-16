using Encamina.Enmarcha.Bot.Abstractions.Adapters;
using Encamina.Enmarcha.Bot.Abstractions.Middlewares;
using Encamina.Enmarcha.Bot.Middlewares;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;

using Microsoft.Bot.Connector.Authentication;

using Microsoft.Bot.Schema;

using Microsoft.Extensions.Logging;

namespace Encamina.Enmarcha.Bot.Adapters;

/// <summary>
/// Base class for bot adapters with custom error handling that implements the Bot Framework Protocol and can
/// be hosted in different cloud environmens, both public and private. <b>This class is abstract.</b>
/// </summary>
public class BotCloudAdapterWithErrorHandlerBase : CloudAdapter
{
    /// <summary>
    /// Default rules to include and use most common bot <see cref="IMiddleware"/>s.
    /// </summary>
    public static readonly IReadOnlyList<IMiddlewareUseRule> DefaultMiddlewareUseRules = new List<IMiddlewareUseRule>()
    {
        new MiddlewareUseRule<TelemetryInitializerMiddleware>() { Order = 10 }, // IMPORTANT - This middleware calls 'TelemetryLoggerMiddleware'. Adding 'TelemetryLoggerMiddleware' as middleware will produce repeated log entries.
        new MiddlewareUseRule<TranscriptLoggerMiddleware>() { Order = 20 },
        new MiddlewareUseRule<ShowTypingMiddleware>() { Order = 30 },
        new MiddlewareUseRule<AutoSaveStateMiddleware>() { Order = 40 },
    }.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="BotCloudAdapterWithErrorHandlerBase"/> class.
    /// </summary>
    /// <param name="adapterOptions">Options for this bot adapter.</param>
    protected BotCloudAdapterWithErrorHandlerBase(IBotAdapterOptions<BotCloudAdapterWithErrorHandlerBase> adapterOptions) : base(adapterOptions?.BotFrameworkAuthentication ?? BotFrameworkAuthenticationFactory.Create(), adapterOptions?.Logger)
    {
        Options = adapterOptions;
        OnTurnError = ErrorHandlerAsync;
    }

    /// <summary>
    /// Gets this adapter's options.
    /// </summary>
    protected virtual IBotAdapterOptions<BotCloudAdapterWithErrorHandlerBase> Options { get; init; }

    /// <summary>
    /// An error handler that can catch exceptions in the middleware or application.
    /// </summary>
    /// <param name="turnContext">The current turn context.</param>
    /// <param name="exception">The catched excetion.</param>
    /// <returns>A task that represents the asynchronous error handling operation.</returns>
    protected virtual async Task ErrorHandlerAsync(ITurnContext turnContext, Exception exception)
    {
        // Send a message to the user
        var errorMessageText = string.Format(Resources.ResponseMessages.ResourceManager.GetStringByCurrentCulture(nameof(Resources.ResponseMessages.GeneralBotErrorResponse)), turnContext.Activity.Id);
        var errorMessage = MessageFactory.Text(errorMessageText, errorMessageText, InputHints.ExpectingInput);
        await turnContext.SendActivityAsync(errorMessage);

        if (Options != null)
        {
            // Send the exception telemetry.
            Options.BotTelemetryClient.TrackException(exception, new Dictionary<string, string> { { @"Bot exception caught in", $"{GetType().Name} - {nameof(OnTurnError)}" } });

            // Log any leaked exception from the application.
            Options.Logger.LogError(exception, Resources.ExceptionMessages.UnhandledException, turnContext.Activity.Id, exception.Message);

            var conversationState = Options.BotStates.OfType<ConversationState>().SingleOrDefault();

            if (conversationState != null)
            {
                try
                {
                    // Delete the conversationState for the current conversation to prevent the
                    // bot from getting stuck in a error-loop caused by being in a bad state.
                    await conversationState.DeleteAsync(turnContext);
                }
                catch (Exception e)
                {
                    Options.Logger.LogError(e, Resources.ExceptionMessages.AttemptingDeleteConversationState, e.Message);
                }
            }
        }

        // Send a trace activity. This information could be displayed in the Bot Framework Emulator for example.
        await turnContext.TraceActivityAsync($@"{nameof(OnTurnError)} Trace", exception.ToString(), "https://www.botframework.com/schemas/error", $@"{nameof(OnTurnError)} - Exception");
    }

    /// <summary>
    /// Initialize common middlewares on a default order and with default conditions as defined by <see cref="DefaultMiddlewareUseRules"/>.
    /// </summary>
    protected void InitializeDefaultMiddlewares()
    {
        InitializeMiddlewares(DefaultMiddlewareUseRules);
    }

    /// <summary>
    /// Initialize common middlewares on a default order.
    /// </summary>
    /// <param name="middlewareUseRules">
    /// A collection of middleware use rules that helps setting the middleware inclusion or usage order, as well as any inclusion condition.
    /// </param>
    protected void InitializeMiddlewares(IEnumerable<IMiddlewareUseRule> middlewareUseRules)
    {
        if (Options?.Middlewares?.Any() ?? false)
        {
            var dicMiddlewares = Options.Middlewares.ToDictionary(i => i.GetType(), i => i);

            foreach (var middlewareUseRule in middlewareUseRules.OrderBy(m => m.Order))
            {
                if (middlewareUseRule.IncludeCondition && dicMiddlewares.TryGetValue(middlewareUseRule.MiddlewareType, out var middleware))
                {
                    Use(middleware);
                }
            }
        }
    }
}
