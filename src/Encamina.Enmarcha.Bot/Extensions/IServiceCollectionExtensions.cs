#pragma warning disable S2360 // Optional parameters should not be used

using System.Globalization;

using Azure.Core;

using Encamina.Enmarcha.AI;

using Encamina.Enmarcha.Bot.Abstractions.Greetings;

using Encamina.Enmarcha.Bot.Greetings;
using Encamina.Enmarcha.Bot.Middlewares;
using Encamina.Enmarcha.Bot.QuestionAnswering;
using Encamina.Enmarcha.Conversation.Abstractions;
using Encamina.Enmarcha.Core.Extensions;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Azure.Blobs;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;

using Microsoft.Bot.Schema;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using ExceptionMessages = Encamina.Enmarcha.Bot.Resources.ExceptionMessages;
using IMiddleware = Microsoft.Bot.Builder.IMiddleware;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring common and required services for bots.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds default bot states.
    /// </summary>
    /// <remarks>
    /// These default bot states are <see cref="ConversationState"/> and <see cref="UserState"/>.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the default bot states.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotDefaultStates(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddBotState<UserState>(serviceLifetime)
                       .AddBotState<ConversationState>(serviceLifetime);
    }

    /// <summary>
    /// Adds a bot state.
    /// </summary>
    /// <typeparam name="TBotState">The bot state type to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the bot state.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">
    /// If there isn't a <see cref="IStorage"/> configured in the service collection.
    /// </exception>
    public static IServiceCollection AddBotState<TBotState>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TBotState : BotState
    {
        return services.Any(s => s.ServiceType == typeof(IStorage))
            ? services.TryAddType<TBotState>(serviceLifetime)
                      .AddType<BotState, TBotState>(serviceLifetime)
            : throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.MissingStorageDependency)));
    }

    /// <summary>
    /// Adds a bot transcript logger middleware as singleton using a memory transcript store.
    /// </summary>
    /// <remarks>This extension method uses the <see cref="MemoryTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTranscriptLoggerMiddlewareWithInMemoryStore(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger, MemoryTranscriptStore>(serviceLifetime)
                       .AddBotTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds a bot transcript logger middleware as singleton using a Blob container as transcript logger (and store).
    /// </summary>
    /// <remarks>This extension method uses the <see cref="BlobsTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="dataConnectionString">Azure Storage connection string.</param>
    /// <param name="containerName">Name of the Blob container where transcripts will be stored. Default is 'bot-transcripts'.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="BlobsTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTranscriptLoggerMiddlewareWithBlobStore(this IServiceCollection services, string dataConnectionString, string containerName = @"bot-transcripts", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, new BlobsTranscriptStore(dataConnectionString, containerName))
                       .AddBotTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds a bot transcript logger middleware as singleton using a specific <see cref="ITranscriptLogger"/>.
    /// </summary>
    /// <typeparam name="TTranscriptLogger">The type of the transcript logger to use.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="transcriptLogger">The transcript logger to use.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTranscriptLoggerMiddlewareWithBlobStore<TTranscriptLogger>(this IServiceCollection services, TTranscriptLogger transcriptLogger, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TTranscriptLogger : ITranscriptLogger
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, transcriptLogger)
                       .AddBotTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds a bot transcript logger middleware as singleton using a file storage as transcript store.
    /// </summary>
    /// <remarks>This extension method uses the <see cref="MemoryTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="folder">A folder path to place the transcript files.</param>
    /// <param name="overwriteTranscriptFiles">
    /// A flag to indicate if transcript files should be overwritten or not. This is usually helpful for unit test scenarios. Default value is <see langword="false"/>.
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTranscriptLoggerMiddlewareWith(this IServiceCollection services, string folder, bool overwriteTranscriptFiles = false, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, new FileTranscriptLogger(folder, overwriteTranscriptFiles))
                       .AddBotTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds in-memory support to store bot's state.
    /// </summary>
    /// <remarks>
    /// Since in-memory storage is cleared each time the bot is restarted, it's best suited for testing purposes and is not intended for production use.
    /// Persistent storage types, such as database storage, are best for production bots.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the in-memory storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStorageInMemory(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IStorage, MemoryStorage>(serviceLifetime);
    }

    /// <summary>
    /// Adds Cosmos DB as bot storage from parameters in configuration.
    /// </summary>
    /// <remarks>
    /// Using Cosmos DB as storage does not automatically create a database within the Cosmos DB account.
    /// It requires creating a new database manually; however, the container will be created automatically thou.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="endpointConfigurationKey">The name of the configuration parameter for the Cosmos DB endpoint value.</param>
    /// <param name="authenticationKeyConfigurationKey">The name of the configuration parameter for the Cosmos DB authentication key.</param>
    /// <param name="databaseIdConfigurationKey">The name of the configuration parameter for the Cosmos DB database unique identifier.</param>
    /// <param name="containerIdConfigurationKey">The name of the configuration parameter fot the Cosmos DB container unique identifier.</param>
    /// <param name="serviceLifetime">The lifetime for the Cosmos DB storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStorageCosmosDb(this IServiceCollection services, IConfiguration configuration, string endpointConfigurationKey, string authenticationKeyConfigurationKey, string databaseIdConfigurationKey, string containerIdConfigurationKey, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddBotStorageCosmosDb(
            configuration.GetValue<string>(endpointConfigurationKey),
            configuration.GetValue<string>(authenticationKeyConfigurationKey),
            configuration.GetValue<string>(databaseIdConfigurationKey),
            configuration.GetValue<string>(containerIdConfigurationKey),
            serviceLifetime);
    }

    /// <summary>
    /// Adds Cosmos DB as bot storage from given parameters.
    /// </summary>
    /// <remarks>
    /// Using Cosmos DB as storage does not automatically create a database within the Cosmos DB account.
    /// It requires creating a new database manually; however, the container will be created automatically thou.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="endpoint">A Cosmos DB endpoint.</param>
    /// <param name="authenticationKey">A Cosmos DB authentication key.</param>
    /// <param name="databaseId">A Cosmos DB database unique identifier.</param>
    /// <param name="containerId">A cosmos DB container unique identifier.</param>
    /// <param name="serviceLifetime">The lifetime for the Cosmos DB storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStorageCosmosDb(this IServiceCollection services, string endpoint, string authenticationKey, string databaseId, string containerId, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IStorage>(serviceLifetime, new CosmosDbPartitionedStorage(new CosmosDbPartitionedStorageOptions()
        {
            CosmosDbEndpoint = endpoint,
            AuthKey = authenticationKey,
            DatabaseId = databaseId,
            ContainerId = containerId,
            CompatibilityMode = false,
        }));
    }

    /// <summary>
    /// Adds Blob Storage as bot storage from parameters in configuration.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="connectionStringConfigurationKey">The name of the configuration parameter with the connection string for the Blob Storage.</param>
    /// <param name="containerNameConfigurationKey">The name of the configuration parameter with the container name for the Blob Storage.</param>
    /// <param name="serviceLifetime">The lifetime for the Blob Storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStorageBlob(this IServiceCollection services, IConfiguration configuration, string connectionStringConfigurationKey, string containerNameConfigurationKey, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddBotStorageBlob(configuration.GetValue<string>(connectionStringConfigurationKey), configuration.GetValue<string>(containerNameConfigurationKey), serviceLifetime);
    }

    /// <summary>
    /// Adds Blob Storage as bot storage from given parameters.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionString">An Azure Storage connection string for the Blob Storage.</param>
    /// <param name="containerName">The name of the Blob container where bot states will be stored. Default is 'bot-states'.</param>
    /// <param name="serviceLifetime">The lifetime for the Blob Storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStorageBlob(this IServiceCollection services, string connectionString, string containerName = @"bot-states", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IStorage>(serviceLifetime, new BlobsStorage(connectionString, containerName));
    }

    /// <summary>
    /// Adds a bot middleware.
    /// </summary>
    /// <typeparam name="TBotMiddleware">The type of a specific bot middleware. It must implement interface <see cref="IMiddleware"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TBotMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotMiddleware<TBotMiddleware>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TBotMiddleware : class, IMiddleware
    {
        return services.TryAddType<TBotMiddleware>(serviceLifetime)
                       .AddType<IMiddleware, TBotMiddleware>(serviceLifetime);
    }

    /// <summary>
    /// Adds a bot middleware from an implementation factory.
    /// </summary>
    /// <typeparam name="TBotMiddleware">The type of a specific bot middleware. It must implement interface <see cref="IMiddleware"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TBotMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotMiddleware<TBotMiddleware>(this IServiceCollection services, Func<IServiceProvider, TBotMiddleware> implementationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TBotMiddleware : class, IMiddleware
    {
        return services.TryAddType(serviceLifetime, implementationFactory)
                       .AddType<IMiddleware, TBotMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetService<TBotMiddleware>()!);
    }

    /// <summary>
    /// Adds default bot telemetry logger middleware and default telemetry initializer telemetry.
    /// </summary>
    /// <remarks>
    /// This will add <see cref="TelemetryLoggerMiddleware"/> as telemetry logger middleware and <see cref="TelemetryInitializerMiddleware"/> as telemetry initializer.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="logPersonalInformation">
    /// If <see langword="true"/> then personally identifiable information will be stored as part of the telemetry, which includes among others the name of the user and
    /// any text message between the user and the bot. Default is <see langword="false"/>.
    /// </param>
    /// <param name="serviceLifetime">The lifetime for <see cref="TelemetryLoggerMiddleware"/> and <see cref="TelemetryInitializerMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotDefaultTelemetryMiddleware(this IServiceCollection services, bool logPersonalInformation = false, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.Any(s => s.ServiceType == typeof(IBotTelemetryClient))
            ? services.TryAddType(serviceLifetime, sp => new TelemetryLoggerMiddleware(sp.GetRequiredService<IBotTelemetryClient>(), logPersonalInformation))
                      .TryAddType<TelemetryInitializerMiddleware>(serviceLifetime)
                      .AddType<IMiddleware>(serviceLifetime, sp => sp.GetRequiredService<TelemetryLoggerMiddleware>())
                      .AddType<IMiddleware>(serviceLifetime, sp => sp.GetRequiredService<TelemetryInitializerMiddleware>())
            : throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.MissingBotTelemetryClientDependency)));
    }

    /// <summary>
    /// Adds the <see cref="ShowTypingMiddleware"/> bot middleware.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="delay">Initial delay before sending first typing indicator. Defaults to 500ms.</param>
    /// <param name="period">Rate at which additional typing indicators will be sent. Defaults to every 2000ms.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="ShowTypingMiddleware"/> bot middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotShowTypingMiddleware(this IServiceCollection services, int delay = 500, int period = 2000, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, new ShowTypingMiddleware(delay, period))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<ShowTypingMiddleware>());
    }

    /// <summary>
    /// Adds the <see cref="AutoSaveStateMiddleware"/> bot middleware.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="AutoSaveStateMiddleware"/> bot middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotAutoSaveStateMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, serviceProvider => new AutoSaveStateMiddleware(serviceProvider.GetServices<BotState>().ToArray()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<AutoSaveStateMiddleware>());
    }

    /// <summary>
    /// Adds answer processor for question answering as singleton.
    /// </summary>
    /// <typeparam name="TQuestionAnsweringAnswerProcessor">The type of answer processor to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TQuestionAnsweringAnswerProcessor"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotQuestionAnsweringAnswersProcessor<TQuestionAnsweringAnswerProcessor>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TQuestionAnsweringAnswerProcessor : SendAnswersProcessorBase
    {
        return services.TryAddType<TQuestionAnsweringAnswerProcessor>(serviceLifetime);
    }

    /// <summary>
    /// Adds bot default telemetry configuration for Application Insights.
    /// </summary>
    /// <remarks>
    /// This default configuration uses <see cref="BotTelemetryClient"/> as default implementation of <see cref="IBotTelemetryClient"/>, also
    /// adding <see cref="OperationCorrelationTelemetryInitializer"/> and <see cref="TelemetryBotIdInitializer"/> as telemetry initializers.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="applicationInsightsConnectionString">
    /// The connection string of the Application Insights for the application.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotDefaultApplicationInsightsTelemetry(this IServiceCollection services, string applicationInsightsConnectionString)
    {
        return services.AddSingleton<IBotTelemetryClient>(new BotTelemetryClient(new TelemetryClient(new TelemetryConfiguration
        {
            ConnectionString = applicationInsightsConnectionString,
        }))).AddSingleton<OperationCorrelationTelemetryInitializer>()
            .AddSingleton<TelemetryBotIdInitializer>();
    }

    /// <summary>
    /// Adds a bot's middleware to translate from detected languages ​​to languages ​​given by parameters, unless the detection language is in an exception list.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="translateToLanguage">The language to translate to.</param>
    /// <param name="languageExceptions">Collection of languages ​​that are the exception to translate.</param>
    /// <param name="languageDetectionServiceName">The name of a language detection cognitive service.</param>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTranslateDetectedLanguageMiddleware(this IServiceCollection services, CultureInfo translateToLanguage, IEnumerable<CultureInfo> languageExceptions, string languageDetectionServiceName, string textTranslationServiceName, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, sp => new DetectedLanguageTranslatorMiddleware(translateToLanguage, languageExceptions, languageDetectionServiceName, textTranslationServiceName, sp.GetRequiredService<ICognitiveServiceProvider>()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<DetectedLanguageTranslatorMiddleware>());
    }

    /// <summary>
    /// Adds a bot's middleware to translate outgoing messages from start activities.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotStartActivityTranslatorMiddleware(this IServiceCollection services, string textTranslationServiceName, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, sp => new StartActivityTranslatorMiddleware(textTranslationServiceName, sp.GetRequiredService<ICognitiveServiceProvider>()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<StartActivityTranslatorMiddleware>());
    }

    /// <summary>
    /// Adds a bot's middleware to save log activity in conversation state.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotConversationStateLoggerMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ConversationStateLoggerMiddleware>(serviceLifetime)
                       .AddType<IMiddleware, ConversationStateLoggerMiddleware>(serviceLifetime);
    }

    /// <summary>
    /// Adds a localized greetings provider based on <see cref="HeroCard">hero cards</see> configured from localized parameters stored in a Table Storage.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message. Default name <c>Greetings</c>.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the greetings provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, string tableConnectionString, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
                       .TryAddType<IGreetingsProvider, LocalizedHeroCardGreetingsProvider>(serviceLifetime)
                       .TryAddType<ILocalizedHeroCardGreetingsOptions>(serviceLifetime, sp => new LocalizedHeroCardGreetingsOptionsFromTableStorage(tableConnectionString, tableName, defaultLocale, cacheAbsoluteExpirationSeconds, sp.GetService<IMemoryCache>()));
    }

    /// <summary>
    /// Adds a localized greetings provider based on <see cref="HeroCard">hero cards</see> configured from localized parameters stored in a Table Storage.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableEndpoint">The Table Storage endpoint URI.</param>
    /// <param name="tokenCredential">The <see cref="TokenCredential"/> to use for authenticating with the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message. Default name <c>Greetings</c>.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the greetings provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, TokenCredential tokenCredential, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .TryAddType<IGreetingsProvider, LocalizedHeroCardGreetingsProvider>(serviceLifetime)
            .TryAddType<ILocalizedHeroCardGreetingsOptions>(serviceLifetime, sp => new LocalizedHeroCardGreetingsOptionsFromTableStorage(tableEndpoint, tokenCredential, tableName, defaultLocale, cacheAbsoluteExpirationSeconds, sp.GetService<IMemoryCache>()));
    }

    /// <summary>
    /// Adds a localized greetings provider based on <see cref="HeroCard">hero cards</see> configured from localized parameters stored in a Table Storage.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableEndpoint">The Table Storage endpoint URI.</param>
    /// <param name="tokenCredentialProvider">The function to provide a <see cref="TokenCredential"/> for authenticating with the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized parameters for the greetings message. Default name <c>Greetings</c>.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the greetings provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, Func<IServiceProvider, TokenCredential> tokenCredentialProvider, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .TryAddType<IGreetingsProvider, LocalizedHeroCardGreetingsProvider>(serviceLifetime)
            .TryAddType<ILocalizedHeroCardGreetingsOptions>(serviceLifetime, sp => new LocalizedHeroCardGreetingsOptionsFromTableStorage(tableEndpoint, tokenCredentialProvider(sp), tableName, defaultLocale, cacheAbsoluteExpirationSeconds, sp.GetService<IMemoryCache>()));
    }

    /// <summary>
    /// Adds a localized greetings provider based on texts responses.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="intentName">The intent name. Defaults to <c>Greetings</c> as intent name.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotLocalizedResponseGreetingsProvider(this IServiceCollection services, string defaultLocale, string intentName = @"Greetings", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IGreetingsProvider, LocalizedResponseGreetingsProvider>(serviceLifetime, sp => new LocalizedResponseGreetingsProvider(sp.GetRequiredService<IIntentResponsesProvider>(), defaultLocale, intentName));
    }

    /// <summary>
    /// Adds a Table Storage as repository for the bot's localized responses.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableConnectionString">The Table Storage connection string.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized responses. Default name <c>Responses</c>.</param>
    /// <param name="intentCounterSeparator">An intent counter separator for scenarios with multiple responses.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the responses provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, string tableConnectionString, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableConnectionString, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    /// <summary>
    /// Adds a Table Storage as repository for the bot's localized responses.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableEndpoint">The Table Storage endpoint URI.</param>
    /// <param name="tokenCredential">The <see cref="TokenCredential"/> to use for authenticating with the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized responses. Default name <c>Responses</c>.</param>
    /// <param name="intentCounterSeparator">An intent counter separator for scenarios with multiple responses.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the responses provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, TokenCredential tokenCredential, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableEndpoint, tokenCredential, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    /// <summary>
    /// Adds a Table Storage as repository for the bot's localized responses.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="defaultLocale">The default locale.</param>
    /// <param name="tableEndpoint">The Table Storage endpoint URI.</param>
    /// <param name="tokenCredentialProvider">The function to provide a <see cref="TokenCredential"/> for authenticating with the Table Storage.</param>
    /// <param name="tableName">The name of the table in the Table storage that contains the localized responses. Default name <c>Responses</c>.</param>
    /// <param name="intentCounterSeparator">An intent counter separator for scenarios with multiple responses.</param>
    /// <param name="cacheAbsoluteExpirationSeconds">
    /// The absolute expiration time, relative to now in seconds for a cache to store values retrieved from the Table Storage, to improve performance. Default <c>86400</c> (i.e., 24 hours - 1 day).
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the responses provider.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddBotTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, Func<IServiceProvider, TokenCredential> tokenCredentialProvider, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableEndpoint, tokenCredentialProvider, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    private static IServiceCollection AddBotTranscriptLoggerMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<TranscriptLoggerMiddleware>(serviceLifetime)
                       .AddType<IMiddleware, TranscriptLoggerMiddleware>(serviceLifetime);
    }
}

#pragma warning restore S2360 // Optional parameters should not be used
