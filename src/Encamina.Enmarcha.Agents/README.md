# Agents

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Agents)](https://www.nuget.org/packages/Encamina.Enmarcha.Agents)

Agents is a project that primarily contains cross-cutting utilities that can be used in a wide variety of projects.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Agents](https://www.nuget.org/packages/Encamina.Enmarcha.Agents) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Agents

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Agents](https://www.nuget.org/packages/Encamina.Enmarcha.Agents) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Agents

## How to use
Below are some of the most important utilities.

## ActivityProcessorBase

`ActivityProcessorBase` is an abstract class that processes activities from a turn context. It extends the `HandlerManagerBase<IActivityHandler>` class and implements the `IActivityProcessor` interface.

### Usage

To use this class, you need to create a derived class and provide the necessary parameters to the base constructor:

```csharp
public class MyActivityProcessor : ActivityProcessorBase
{
    public MyActivityProcessor(IEnumerable<IActivityHandler> handlers) : base(handlers)
    {
    }

    // Implement other methods as needed
}
```

#### Methods

The class defines several methods:

- `BeginProcessAsync`: Begins the process of an activity.
- `EndProcessAsync`: Ends the process of an activity.
- `ProcessAsync`: Processes an activity based on the HandlerProcessTimes parameter.

## Adapters

### ChannelCloudAdapterWithErrorHandlerBase

`ChannelCloudAdapterWithErrorHandlerBase` is a class that extends the `CloudAdapter` from the Agents Framework, providing custom error handling for an agent that can be hosted in different cloud environments.

#### Usage

To use this class, you need to create an instance of `IChannelAdapterOptions<ChannelCloudAdapterWithErrorHandlerBase>` and pass it to the constructor:

```csharp
var options = new ChannelAdapterOptions<ChannelCloudAdapterWithErrorHandlerBase>(//...)
{   
    Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ChannelCloudAdapterWithErrorHandlerBase>()
};

var adapter = new ChannelCloudAdapterWithErrorHandlerBase(options);
```

This will create a new instance of ChannelCloudAdapterWithErrorHandlerBase with the provided options. The OnTurnError property is set to ErrorHandlerAsync, which handles errors during the agent's turn.

#### Middleware
The class also defines a list of default middleware rules that the agent will use:

`TelemetryInitializerMiddleware`: This middleware initializes telemetry for the agent. It's important to note that this middleware calls
`TelemetryLoggerMiddleware`, so adding `TelemetryLoggerMiddleware` as middleware will produce repeated log entries.
`TranscriptLoggerMiddleware`: This middleware logs the transcript of the conversation.
`ShowTypingMiddleware`: This middleware sends typing indicators to the user while the agent is processing the message.
`AutoSaveStateMiddleware`: This middleware automatically saves the agent's state after each turn.

### ChannelAdapterOptionsBase

`ChannelAdapterOptionsBase` is an abstract class that provides common options for an channel adapter. It implements the `IChannelAdapterOptions` interface.

### Usage

To use this class, you need to create a derived class and provide the necessary parameters to the base constructor:

```csharp
public class MyChannelAdapterOptions : ChannelAdapterOptionsBase
{
    public MyChannelAdapterOptions(IActivityTaskQueue activityTaskQueue, IChannelServiceClientFactory channelServiceClientFactory,
        IAgentTelemetryClient agentTelemetryClient, IEnumerable<AgentState> agentStates, IEnumerable<IMiddleware> agentMiddlewares)
        :  base(activityTaskQueue, channelServiceClientFactory, agentTelemetryClient, agentStates, agentMiddlewares)
    {
    }
}
```

## Cards

### CardActionFactory

`CardActionFactory` is a static class that contains utility methods to create various `CardAction` objects. `CardAction` is a class from the Agents Framework that represents an action that can be performed from a card.

#### Usage

You can use the methods of this class to create `CardAction` objects:

```csharp
var emptyAction = CardActionFactory.EmptyAction("Some text");

var value = new MyActivityValue(); // MyActivityValue must inherit from ActivityValueBase
var messageBackAction = CardActionFactory.MessageBackAction(value, "Some text", "https://example.com/image.png");
```

#### Methods
The class defines several methods:

- `EmptyAction`: Creates a CardAction without an action type and only text.
- `MessageBackAction`: Creates a CardAction that can be used to message back the agent with some values.

## Controllers

### AgentBaseController

`AgentBaseController` is an abstract class that extends the `ControllerBase` class from ASP.NET. It's designed to handle requests from web chats or DirectLine clients.

### Usage

To use this class, you need to create a derived class and provide the necessary parameters to the base constructor:

```csharp
public class MyAgentController : AgentBaseController
{
    public MyAgentController(IAgentHttpAdapter adapter, IAgent agent) : base(adapter, agent)
    {
    }

    public override async Task HandleAsync()
    {
        // Implement your own logic to handle the request
    }
}
```

You can then use your derived controller in your ASP.NET application:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAgent<MyAgent>();
    services.AddControllers();
}
```

### Methods
The class defines a method:

- `HandleAsync`: Handles a request for the agent. This method is virtual and can be overridden in a derived class.

## Extensions - ILoggingBuilderExtensions

`ILoggingBuilderExtensions` is a static class that contains an extension method for the `ILoggingBuilder` interface. It's designed to add an Application Insights logger to the logging factory.

### Usage

You can use the `AddApplicationInsightsConversationScoped` method to add an Application Insights logger to your logging factory:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder.AddApplicationInsightsConversationScoped(
            configureTelemetryConfiguration: config => { /* Configure telemetry here */ },
            configureApplicationInsightsLoggerOptions: options => { /* Configure logger options here */ }
        );
    });
}
```

### Methods

The class defines a method:

- `AddApplicationInsightsConversationScoped`: Adds an Application Insights logger named ApplicationInsightsConversationScopedLoggerProvider to the factory.

## Extensions - IServiceCollectionExtensions

`IServiceCollectionExtensions` is a static class that contains extension methods for the `IServiceCollection` interface. It's designed to add services to the service collection, which is a fundamental part of the dependency injection system in ASP.NET.

### Usage

You can use the `AddDefaultAgentStates` method to add default agent states to your service collection:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDefaultAgentStates();
}
```

### Methods

The class defines a method:

- `AddDefaultAgentStates`: Adds default agent states to the service collection. These default agent states are `ConversationState` and `UserState`.

## Greetings

### LocalizedHeroCardGreetingsOptionsFromTableStorage

`LocalizedHeroCardGreetingsOptionsFromTableStorage` is a class that provides localized options for greeting messages based on hero cards. The data is stored in Azure Table Storage. It implements the `ILocalizedHeroCardGreetingsOptions` interface.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
var tableConnectionString = "Your Azure Table Storage connection string";
var tableName = "Your Azure Table Storage table name";
var defaultLocale = "en-US"; // The default locale
var cacheAbsoluteExpirationSeconds = 86400; // The cache expiration time in seconds
IMemoryCache memoryCache = null; // An optional memory cache

var options = new LocalizedHeroCardGreetingsOptionsFromTableStorage(tableConnectionString, tableName, defaultLocale, cacheAbsoluteExpirationSeconds, memoryCache);
```

You can then use this instance to get localized greeting messages based on hero cards. The exact usage will depend on how the ILocalizedHeroCardGreetingsOptions interface and its methods are used in your application.

#### Constructor
The class has a constructor:

- `LocalizedHeroCardGreetingsOptionsFromTableStorage`: Initializes a new instance of the class. It takes a connection string and table name for Azure Table Storage, a default locale, an expiration time for the cache, and an optional memory cache.

### LocalizedHeroCardGreetingsProvider

`LocalizedHeroCardGreetingsProvider` is a class that sends a greeting message using a `HeroCard`. The message is localized based on the culture info from the activity. It extends the `GreetingsProviderBase` class.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
var options = new LocalizedHeroCardGreetingsOptionsFromTableStorage(/* parameters here */);
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LocalizedHeroCardGreetingsProvider>();

var provider = new LocalizedHeroCardGreetingsProvider(options, logger);
```
You can then use this instance to send a greeting message:
```csharp
await provider.SendAsync(turnContext, cancellationToken);
```

#### Constructor and Properties

- `LocalizedHeroCardGreetingsProvider`: Initializes a new instance of the class. It takes greetings options with values to build a HeroCard and a logger for this greetings provider.
- `Options`: Gets the current greetings options with values to build a HeroCard.
- 
#### Methods

- `SendAsync`: Sends a greeting message using a `HeroCard`. The message is localized based on the culture info from the activity.


### LocalizedResponseGreetingsProvider

`LocalizedResponseGreetingsProvider` is a class that sends a greeting message based on responses retrieved from an `IIntentResponsesProvider`. The message is localized based on the culture info from the activity. It extends the `GreetingsProviderBase` class.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
IIntentResponsesProvider responsesProvider = /* Your IIntentResponsesProvider instance */;
string defaultLocale = "en-US"; // The default locale
string intentName = "Greetings"; // The expected greetings intent name

var provider = new LocalizedResponseGreetingsProvider(responsesProvider, defaultLocale, intentName);
```

You can then use this instance to send a greeting message:
```csharp
await provider.SendAsync(turnContext, cancellationToken);
```

#### Constructor

- `LocalizedResponseGreetingsProvider`: Initializes a new instance of the class. It takes an `IIntentResponsesProvider`, a default locale, and an intent name.
 
#### Methods

- `SendAsync`: Sends a greeting message based on responses retrieved from an `IIntentResponsesProvider`. The message is localized based on the culture info from the activity.

## Logging

### ApplicationInsightsConversationScopedLoggerEventSource

`ApplicationInsightsConversationScopedLoggerEventSource` is a class that logs error messages to Application Insights. It extends the `EventSource` class.

### Usage

You can use the `FailedToLog` method to log an error message to Application Insights:

```csharp
string errorMessage = "An error occurred.";
string applicationName = "MyApplication"; // Optional

ApplicationInsightsConversationScopedLoggerEventSource.Log.FailedToLog(errorMessage, applicationName);
```

#### Constructor and Properties

- `ApplicationInsightsConversationScopedLoggerEventSource`: Initializes a new instance of the class. It initializes the `applicationName` field with the name of the entry assembly.
- `Log`: Gets the instance of the logger event source.

#### Methods

- `FailedToLog`: Logs an error message to Application Insights. It takes an error message and an optional application name as parameters.

### ApplicationInsightsConversationScopedLogger

`ApplicationInsightsConversationScopedLogger` is a class that forwards log messages as Application Insights trace events and tracks the `ConversationId` as a property in the telemetry. It implements the `ILogger` interface.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
string categoryName = "MyCategory";
TelemetryClient telemetryClient = new TelemetryClient();
ApplicationInsightsConversationScopedLoggerOptions options = new ApplicationInsightsConversationScopedLoggerOptions();
IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

var logger = new ApplicationInsightsConversationScopedLogger(categoryName, telemetryClient, options, httpContextAccessor);
```
You can then use this instance to log messages:

```csharp
logger.Log(LogLevel.Information, "This is a log message.");
```

#### Constructor

- `ApplicationInsightsConversationScopedLogger`: Initializes a new instance of the class. It takes a category name, a TelemetryClient, an ApplicationInsightsConversationScopedLoggerOptions, and an IHttpContextAccessor.

#### ILogger Methods
As an implementation of the `ILogger` interface, this class should implement methods such as `Log`, `IsEnabled`, and `BeginScope`. 

### ApplicationInsightsConversationScopedLoggerProvider

`ApplicationInsightsConversationScopedLoggerProvider` is a class that creates instances of `ApplicationInsightsConversationScopedLogger`. It implements the `ILoggerProvider` and `ISupportExternalScope` interfaces.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
IOptions<ApplicationInsightsConversationScopedLoggerOptions> options = Options.Create(new ApplicationInsightsConversationScopedLoggerOptions());
TelemetryClient telemetryClient = new TelemetryClient();
IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

var loggerProvider = new ApplicationInsightsConversationScopedLoggerProvider(options, telemetryClient, httpContextAccessor);
```

You can then use this instance to create a logger:

```csharp
ILogger logger = loggerProvider.CreateLogger("MyCategory");
```

#### Constructor

- `ApplicationInsightsConversationScopedLoggerProvider`: Initializes a new instance of the class. It takes an `IOptions<ApplicationInsightsConversationScopedLoggerOptions>`, a `TelemetryClient`, and an `IHttpContextAccessor`.
  
#### ILoggerProvider Methods

As an implementation of the `ILoggerProvider` interface, this class should implement methods such as `CreateLogger` and `Dispose`.

## Middlewares

### ConversationStateLoggerMiddleware

`ConversationStateLoggerMiddleware` is a middleware that automatically saves logger in conversation state. It implements the `IMiddleware` interface.

#### Usage

To use this middleware, you need to create an instance and provide the necessary parameters:

```csharp
ConversationState conversationState = new ConversationState(new MemoryStorage());

var middleware = new ConversationStateLoggerMiddleware(conversationState);
```

You can then add this middleware to your agent:

```csharp
agentAdapter.Use(middleware);
```

GitHub Copilot: This C# code defines a class `ConversationStateLoggerMiddleware` that implements the `IMiddleware` interface. This middleware is designed to automatically save logger in conversation state.

Here's a brief explanation of the key components:

- `ConversationStateLoggerMiddleware`: This is the constructor for the class. It takes a `ConversationState` as a parameter, which is used to initialize the `conversationState` field.

- `OnTurnAsync`: This method saves the activity of the context in `AgentState` by conversation id. It takes a `ITurnContext`, a `NextDelegate`, and an optional `CancellationToken` as parameters.

Here's how you might include this in a README:

### ConversationStateLoggerMiddleware

`ConversationStateLoggerMiddleware` is a middleware that automatically saves logger in conversation state. It implements the `IMiddleware` interface.

#### Usage

To use this middleware, you need to create an instance and provide the necessary parameters:

```csharp
ConversationState conversationState = new ConversationState(new MemoryStorage());

var middleware = new ConversationStateLoggerMiddleware(conversationState);
```

You can then add this middleware to your agent:

```csharp
agentAdapter.Use(middleware);
```

#### Constructor

- `ConversationStateLoggerMiddleware`: Initializes a new instance of the class. It takes a `ConversationState`.

#### IMiddleware Methods

As an implementation of the `IMiddleware` interface, this class should implement methods such as `OnTurnAsync`. This method saves the activity of the context in `AgentState` by conversation id.

### DetectedLanguageTranslatorMiddleware

`DetectedLanguageTranslatorMiddleware` is a middleware that automatically translates from a detected language to a configured translation language. It implements the `IMiddleware` interface.

#### Usage

To use this middleware, you need to create an instance and provide the necessary parameters:

```csharp
CultureInfo translateToLanguage = new CultureInfo("en-US");
IEnumerable<CultureInfo> languageExceptions = new List<CultureInfo> { new CultureInfo("fr-FR") };
string languageDetectionServiceName = "YourLanguageDetectionServiceName";
string textTranslationServiceName = "YourTextTranslationServiceName";
ICognitiveServiceProvider cognitiveServiceProvider = new YourCognitiveServiceProvider();

var middleware = new DetectedLanguageTranslatorMiddleware(translateToLanguage, languageExceptions, languageDetectionServiceName, textTranslationServiceName, cognitiveServiceProvider);
```

You can then add this middleware to your agent:

```csharp
agentAdapter.Use(middleware);
```

#### Constructor

- `DetectedLanguageTranslatorMiddleware`: Initializes a new instance of the class. It takes a `CultureInfo` for the language to translate to, an `IEnumerable<CultureInfo>` for languages that are exceptions to translate, the names of a language detection cognitive service and a text translation cognitive service, and an `ICognitiveServiceProvider` to locate these services.

#### IMiddleware Methods

As an implementation of the `IMiddleware` interface, this class should implement methods such as `OnTurnAsync`.


### StartActivityTranslatorMiddleware

`StartActivityTranslatorMiddleware` is a middleware that automatically translates messages sent to consumers during start activities into the language received as `Activity.Locale`. It implements the `IMiddleware` interface.

#### Usage

To use this middleware, you need to create an instance and provide the necessary parameters:

```csharp
string textTranslationServiceName = "YourTextTranslationServiceName";
ICognitiveServiceProvider cognitiveServiceProvider = new YourCognitiveServiceProvider();

var middleware = new StartActivityTranslatorMiddleware(textTranslationServiceName, cognitiveServiceProvider);
```

You can then add this middleware to your agent:

```csharp
agentAdapter.Use(middleware);
```

#### Constructors

- `StartActivityTranslatorMiddleware`: Initializes a new instance of the class. It takes a `textTranslationServiceName` and an `ICognitiveServiceProvider` to locate a language detection and a text translation service from the given names.

- `StartActivityTranslatorMiddleware`: Initializes a new instance of the class. It takes an `ITextTranslationService` to translate texts.

#### IMiddleware Methods

As an implementation of the `IMiddleware` interface, this class should implement methods such as `OnTurnAsync`.

### TranslatorUtils

`TranslatorUtils` is a static class that provides utility methods for translation operations.

#### Usage

You can use the methods in this class as follows:

```csharp
IDictionary<string, string> translations = new Dictionary<string, string> { { "en-US", "Hello" } };
CultureInfo language = new CultureInfo("en-US");

string translation = TranslatorUtils.GetTranslation(translations, language, "Default text");

ITextTranslationService translationService = new YourTextTranslationService();
IList<Activity> activities = new List<Activity> { new Activity { Type = ActivityTypes.Message, Text = "Hola" } };
CultureInfo fromLanguage = new CultureInfo("es-ES");
CultureInfo toLanguage = new CultureInfo("en-US");
CancellationToken cancellationToken = new CancellationToken();

await TranslatorUtils.TranslateMessagesAsync(translationService, activities, fromLanguage, toLanguage, cancellationToken);

```

#### Methods

- `GetTranslation`: Retrieves a translation from a dictionary of translations based on the provided language. If no translation is found, it returns a default text.

- `TranslateMessagesAsync`: Translates message type activities from one language to another using a provided `ITextTranslationService`. It also handles cancellation through a `CancellationToken`.

## Options

### ApplicationInsightsConversationScopedLoggerOptions

`ApplicationInsightsConversationScopedLoggerOptions` is a class that customizes the behavior of the tracing information sent to Application Insights using `ApplicationInsightsConversationScopedLogger`. It inherits from `ApplicationInsightsLoggerOptions`.

#### Usage

To use this class, you need to create an instance and optionally set the `EventsToTrack`:

```csharp
var options = new ApplicationInsightsConversationScopedLoggerOptions
{
    EventsToTrack = new List<string> { "Event1", "Event2" }
};
```

#### Constructor

- `ApplicationInsightsConversationScopedLoggerOptions`: Initializes a new instance of the class. It doesn't take any parameters.

#### Properties

- `EventsToTrack`: Gets or sets a list of event names that should be tracked. If null, all events will be tracked.

## Question Answering

### SendAnswersProcessorBase

`SendAnswersProcessorBase` is an abstract class that processes and handles a collection of answers. It inherits from `OrderableHandlerManagerBase<ISendAnswersHandler>` and implements `ISendAnswersProcessor` interface.

#### Usage

To use this class, you need to create a derived class and provide the necessary implementations:

```csharp
public class MySendAnswersProcessor : SendAnswersProcessorBase
{
    public MySendAnswersProcessor(IEnumerable<ISendAnswersHandler> handlers) : base(handlers)
    {
    }

    public override Task<SendResponseResult> SendResponseAsync<TAnswer>(ITurnContext context, IEnumerable<TAnswer> answers, CancellationToken cancellationToken)
    {
        // Your implementation here
    }
}
```

#### Constructor

- `SendAnswersProcessorBase`: Initializes a new instance of the class. It takes an `IEnumerable<ISendAnswersHandler>` as a parameter, which is a collection of answer handlers for this processor.

#### Methods

- `SendResponseAsync`: Sends a response by processing and handling a given collection of answers. It takes an `ITurnContext`, an `IEnumerable<TAnswer>`, and a `CancellationToken` as parameters. It returns a `Task<SendResponseResult>`.

### SimpleAnswersHandler

`SimpleAnswersHandler` is a class that sends the first answer found with the highest confidence score as the response. It implements the `ISendAnswersHandler` interface.

#### Usage

To use this class, you need to create an instance:

```csharp
var handler = new SimpleAnswersHandler();
ITurnContext context = new YourTurnContext();
IEnumerable<IAnswer> answers = new List<IAnswer> { new YourAnswer() };
CancellationToken cancellationToken = new CancellationToken();

SendResponseResult result = await handler.HandleSendResponseAsync(context, answers, cancellationToken);
```

### Constructor

- `SimpleAnswersHandler`: Initializes a new instance of the class. It doesn't take any parameters.

### Properties

- `Order`: Ensures that this is the last response handler to be evaluated.

### Methods

- `HandleSendResponseAsync`: Sends a response by handling a given collection of answers. They take an `ITurnContext`, an `IEnumerable<TAnswer>`, and a `CancellationToken` as parameters. They return a `Task<SendResponseResult>`.