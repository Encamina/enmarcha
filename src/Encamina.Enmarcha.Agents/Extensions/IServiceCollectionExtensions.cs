#pragma warning disable S2360 // Optional parameters should not be used

using System.Globalization;

using Azure.Core;

using Encamina.Enmarcha.Agents.Abstractions.Greetings;
using Encamina.Enmarcha.Agents.Abstractions.Telemetry;
using Encamina.Enmarcha.Agents.Greetings;
using Encamina.Enmarcha.Agents.Middlewares;
using Encamina.Enmarcha.Agents.QuestionAnswering;
using Encamina.Enmarcha.Agents.Resources;
using Encamina.Enmarcha.Agents.Telemetry;
using Encamina.Enmarcha.AI;
using Encamina.Enmarcha.Conversation.Abstractions;
using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.Compat;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Storage;
using Microsoft.Agents.Storage.Blobs;
using Microsoft.Agents.Storage.CosmosDb;
using Microsoft.Agents.Storage.Transcript;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring common and required services for agents.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds default agent states.
    /// </summary>
    /// <remarks>
    /// These default agent states are <see cref="ConversationState"/> and <see cref="UserState"/>.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the default agent states.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentDefaultStates(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddAgentState<UserState>(serviceLifetime)
                       .AddAgentState<ConversationState>(serviceLifetime);
    }

    /// <summary>
    /// Adds an agent state.
    /// </summary>
    /// <typeparam name="TAgentState">The agent state type to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the agent state.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">
    /// If there isn't a <see cref="IStorage"/> configured in the service collection.
    /// </exception>
    public static IServiceCollection AddAgentState<TAgentState>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton) where TAgentState : AgentState
    {
        return services.Any(s => s.ServiceType == typeof(IStorage))
            ? services.TryAddType<TAgentState>(serviceLifetime)
                      .AddType<AgentState, TAgentState>(serviceLifetime)
                      .AddType<IAgentState, TAgentState>(serviceLifetime, sp => sp.GetService<TAgentState>()!)
            : throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.MissingStorageDependency)));
    }

    /// <summary>
    /// Adds an agent transcript logger middleware as singleton using a memory transcript store.
    /// </summary>
    /// <remarks>This extension method uses the <see cref="MemoryTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentTranscriptLoggerMiddlewareWithInMemoryStore(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger, MemoryTranscriptStore>(serviceLifetime)
                       .AddAgentTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds an agent transcript logger middleware as singleton using a Blob container as transcript logger (and store).
    /// </summary>
    /// <remarks>This extension method uses the <see cref="BlobsTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="dataConnectionString">Azure Storage connection string.</param>
    /// <param name="containerName">Name of the Blob container where transcripts will be stored. Default is 'bot-transcripts'.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="BlobsTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentTranscriptLoggerMiddlewareWithBlobStore(this IServiceCollection services, string dataConnectionString, string containerName = @"bot-transcripts", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, new BlobsTranscriptStore(dataConnectionString, containerName))
                       .AddAgentTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds an agent transcript logger middleware as singleton using a specific <see cref="ITranscriptLogger"/>.
    /// </summary>
    /// <typeparam name="TTranscriptLogger">The type of the transcript logger to use.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="transcriptLogger">The transcript logger to use.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentTranscriptLoggerMiddlewareWithBlobStore<TTranscriptLogger>(this IServiceCollection services, TTranscriptLogger transcriptLogger, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TTranscriptLogger : ITranscriptLogger
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, transcriptLogger)
                       .AddAgentTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds an agent transcript logger middleware as singleton using a file storage as transcript store.
    /// </summary>
    /// <remarks>This extension method uses the <see cref="MemoryTranscriptStore"/> transcript store.</remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="folder">A folder path to place the transcript files.</param>
    /// <param name="overwriteTranscriptFiles">
    /// A flag to indicate if transcript files should be overwritten or not. This is usually helpful for unit test scenarios. Default value is <see langword="false"/>.
    /// </param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="MemoryTranscriptStore"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentTranscriptLoggerMiddlewareWith(this IServiceCollection services, string folder, bool overwriteTranscriptFiles = false, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<ITranscriptLogger>(serviceLifetime, new FileTranscriptLogger(folder, overwriteTranscriptFiles))
                       .AddAgentTranscriptLoggerMiddleware(serviceLifetime);
    }

    /// <summary>
    /// Adds in-memory support to store agent's state.
    /// </summary>
    /// <remarks>
    /// Since in-memory storage is cleared each time the agent is restarted, it's best suited for testing purposes and is not intended for production use.
    /// Persistent storage types, such as database storage, are best for production agents.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the in-memory storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentStorageInMemory(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IStorage, MemoryStorage>(serviceLifetime);
    }

    /// <summary>
    /// Adds Cosmos DB as agent storage from parameters in configuration.
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
    public static IServiceCollection AddAgentStorageCosmosDb(this IServiceCollection services, IConfiguration configuration, string endpointConfigurationKey, string authenticationKeyConfigurationKey, string databaseIdConfigurationKey, string containerIdConfigurationKey, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddAgentStorageCosmosDb(
            configuration.GetValue<string>(endpointConfigurationKey)!,
            configuration.GetValue<string>(authenticationKeyConfigurationKey)!,
            configuration.GetValue<string>(databaseIdConfigurationKey)!,
            configuration.GetValue<string>(containerIdConfigurationKey)!,
            serviceLifetime);
    }

    /// <summary>
    /// Adds Cosmos DB as agent storage from given parameters.
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
    public static IServiceCollection AddAgentStorageCosmosDb(this IServiceCollection services, string endpoint, string authenticationKey, string databaseId, string containerId, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
    /// Adds Blob Storage as agent storage from parameters in configuration.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="connectionStringConfigurationKey">The name of the configuration parameter with the connection string for the Blob Storage.</param>
    /// <param name="containerNameConfigurationKey">The name of the configuration parameter with the container name for the Blob Storage.</param>
    /// <param name="serviceLifetime">The lifetime for the Blob Storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentStorageBlob(this IServiceCollection services, IConfiguration configuration, string connectionStringConfigurationKey, string containerNameConfigurationKey, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddAgentStorageBlob(configuration.GetValue<string>(connectionStringConfigurationKey)!, configuration.GetValue<string>(containerNameConfigurationKey)!, serviceLifetime);
    }

    /// <summary>
    /// Adds Blob Storage as agent storage from given parameters.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="connectionString">An Azure Storage connection string for the Blob Storage.</param>
    /// <param name="containerName">The name of the Blob container where agent states will be stored. Default is 'bot-states'.</param>
    /// <param name="serviceLifetime">The lifetime for the Blob Storage.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentStorageBlob(this IServiceCollection services, string connectionString, string containerName = @"bot-states", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IStorage>(serviceLifetime, new BlobsStorage(connectionString, containerName));
    }

    /// <summary>
    /// Adds an agent middleware.
    /// </summary>
    /// <typeparam name="TAgentMiddleware">The type of specific agent middleware. It must implement interface <see cref="IMiddleware"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TAgentMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentMiddleware<TAgentMiddleware>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TAgentMiddleware : class, IMiddleware
    {
        return services.TryAddType<TAgentMiddleware>(serviceLifetime)
                       .AddType<IMiddleware, TAgentMiddleware>(serviceLifetime);
    }

    /// <summary>
    /// Adds an agent middleware from an implementation factory.
    /// </summary>
    /// <typeparam name="TAgentMiddleware">The type of specific agent middleware. It must implement interface <see cref="IMiddleware"/>.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="implementationFactory">The factory that creates the service.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TAgentMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentMiddleware<TAgentMiddleware>(this IServiceCollection services, Func<IServiceProvider, TAgentMiddleware> implementationFactory, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TAgentMiddleware : class, IMiddleware
    {
        return services.TryAddType(serviceLifetime, implementationFactory)
                       .AddType<IMiddleware, TAgentMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetService<TAgentMiddleware>()!);
    }

    /// <summary>
    /// Adds default agent telemetry logger middleware and default telemetry initializer telemetry.
    /// </summary>
    /// <remarks>
    /// This will add <see cref="TelemetryLoggerMiddleware"/> as telemetry logger middleware and <see cref="TelemetryInitializerMiddleware"/> as telemetry initializer.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="logPersonalInformation">
    /// If <see langword="true"/> then personally identifiable information will be stored as part of the telemetry, which includes among others the name of the user and
    /// any text message between the user and the agent. Default is <see langword="false"/>.
    /// </param>
    /// <param name="serviceLifetime">The lifetime for <see cref="TelemetryLoggerMiddleware"/> and <see cref="TelemetryInitializerMiddleware"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentDefaultTelemetryMiddleware(this IServiceCollection services, bool logPersonalInformation = false, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.Any(s => s.ServiceType == typeof(IAgentTelemetryClient))
            ? services.TryAddType(serviceLifetime, sp => new TelemetryLoggerMiddleware(sp.GetRequiredService<IAgentTelemetryClient>(), logPersonalInformation))
                      .TryAddType<TelemetryInitializerMiddleware>(serviceLifetime)
                      .AddType<IMiddleware>(serviceLifetime, sp => sp.GetRequiredService<TelemetryLoggerMiddleware>())
                      .AddType<IMiddleware>(serviceLifetime, sp => sp.GetRequiredService<TelemetryInitializerMiddleware>())
            : throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.MissingAgentTelemetryClientDependency)));
    }

    /// <summary>
    /// Adds the <see cref="ShowTypingMiddleware"/> agent middleware.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="delay">Initial delay before sending first typing indicator. Defaults to 500ms.</param>
    /// <param name="period">Rate at which additional typing indicators will be sent. Defaults to every 2000ms.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="ShowTypingMiddleware"/> agent middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentShowTypingMiddleware(this IServiceCollection services, int delay = 500, int period = 2000, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, new ShowTypingMiddleware(delay, period))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<ShowTypingMiddleware>());
    }

    /// <summary>
    /// Adds the <see cref="AutoSaveStateMiddleware"/> agent middleware.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <see cref="AutoSaveStateMiddleware"/> agent middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentAutoSaveStateMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, serviceProvider => new AutoSaveStateMiddleware(serviceProvider.GetServices<IAgentState>().ToArray()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<AutoSaveStateMiddleware>());
    }

    /// <summary>
    /// Adds answer processor for question answering as singleton.
    /// </summary>
    /// <typeparam name="TQuestionAnsweringAnswerProcessor">The type of answer processor to add.</typeparam>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the <typeparamref name="TQuestionAnsweringAnswerProcessor"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentQuestionAnsweringAnswersProcessor<TQuestionAnsweringAnswerProcessor>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        where TQuestionAnsweringAnswerProcessor : SendAnswersProcessorBase
    {
        return services.TryAddType<TQuestionAnsweringAnswerProcessor>(serviceLifetime);
    }

    /// <summary>
    /// Adds agent default telemetry configuration for Application Insights.
    /// </summary>
    /// <remarks>
    /// This default configuration uses <see cref="ApplicationInsightsAgentTelemetryClient"/> as default implementation of <see cref="IAgentTelemetryClient"/>, also
    /// adding <see cref="OperationCorrelationTelemetryInitializer"/> and <see cref="TelemetryAgentIdInitializer"/> as telemetry initializers.
    /// </remarks>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="applicationInsightsConnectionString">
    /// The connection string of the Application Insights for the application.
    /// </param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentDefaultApplicationInsightsTelemetry(this IServiceCollection services, string applicationInsightsConnectionString)
    {
        return services.AddSingleton<IAgentTelemetryClient>(new ApplicationInsightsAgentTelemetryClient(new TelemetryClient(new TelemetryConfiguration
        {
            ConnectionString = applicationInsightsConnectionString,
        }))).AddSingleton<OperationCorrelationTelemetryInitializer>()
            .AddSingleton<TelemetryAgentIdInitializer>();
    }

    /// <summary>
    /// Adds an agent's middleware to translate from detected languages ​​to languages ​​given by parameters, unless the detection language is in an exception list.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="translateToLanguage">The language to translate to.</param>
    /// <param name="languageExceptions">Collection of languages ​​that are the exception to translate.</param>
    /// <param name="languageDetectionServiceName">The name of a language detection cognitive service.</param>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentTranslateDetectedLanguageMiddleware(this IServiceCollection services, CultureInfo translateToLanguage, IEnumerable<CultureInfo> languageExceptions, string languageDetectionServiceName, string textTranslationServiceName, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, sp => new DetectedLanguageTranslatorMiddleware(translateToLanguage, languageExceptions, languageDetectionServiceName, textTranslationServiceName, sp.GetRequiredService<ICognitiveServiceProvider>()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<DetectedLanguageTranslatorMiddleware>());
    }

    /// <summary>
    /// Adds an agent's middleware to translate outgoing messages from start activities.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="textTranslationServiceName">The name of a text translation cognitive service.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentStartActivityTranslatorMiddleware(this IServiceCollection services, string textTranslationServiceName, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType(serviceLifetime, sp => new StartActivityTranslatorMiddleware(textTranslationServiceName, sp.GetRequiredService<ICognitiveServiceProvider>()))
                       .AddType<IMiddleware>(serviceLifetime, serviceProvider => serviceProvider.GetRequiredService<StartActivityTranslatorMiddleware>());
    }

    /// <summary>
    /// Adds an agent's middleware to save log activity in conversation state.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="serviceLifetime">The lifetime for the middleware.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAgentConversationStateLoggerMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
    public static IServiceCollection AddAgentLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, string tableConnectionString, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
    public static IServiceCollection AddAgentLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, TokenCredential tokenCredential, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
    public static IServiceCollection AddAgentLocalizedHeroCardGreetingsProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, Func<IServiceProvider, TokenCredential> tokenCredentialProvider, string tableName = @"Greetings", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
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
    public static IServiceCollection AddAgentLocalizedResponseGreetingsProvider(this IServiceCollection services, string defaultLocale, string intentName = @"Greetings", ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<IGreetingsProvider, LocalizedResponseGreetingsProvider>(serviceLifetime, sp => new LocalizedResponseGreetingsProvider(sp.GetRequiredService<IIntentResponsesProvider>(), defaultLocale, intentName));
    }

    /// <summary>
    /// Adds a Table Storage as repository for the agent's localized responses.
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
    public static IServiceCollection AddAgentTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, string tableConnectionString, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableConnectionString, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    /// <summary>
    /// Adds a Table Storage as repository for the agent's localized responses.
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
    public static IServiceCollection AddAgentTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, TokenCredential tokenCredential, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableEndpoint, tokenCredential, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    /// <summary>
    /// Adds a Table Storage as repository for the agent's localized responses.
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
    public static IServiceCollection AddAgentTableStorageResponsesProvider(this IServiceCollection services, string defaultLocale, Uri tableEndpoint, Func<IServiceProvider, TokenCredential> tokenCredentialProvider, string tableName = @"Responses", string intentCounterSeparator = @"-", double cacheAbsoluteExpirationSeconds = 86400, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddMemoryCache()
            .AddTableStorageResponsesProvider(defaultLocale, tableEndpoint, tokenCredentialProvider, tableName, intentCounterSeparator, cacheAbsoluteExpirationSeconds, serviceLifetime);
    }

    private static IServiceCollection AddAgentTranscriptLoggerMiddleware(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.TryAddType<TranscriptLoggerMiddleware>(serviceLifetime)
                       .AddType<IMiddleware, TranscriptLoggerMiddleware>(serviceLifetime);
    }
}

#pragma warning restore S2360 // Optional parameters should not be used
