# Semantic Kernel - Chat Plugin

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.SemanticKernel.Plugins.Chat)](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Chat)

Chat Plugin is a project that provides Chat functionality in the form of a Semantic Kernel Plugin. It allows users to interact while chatting and asking questions to an Artificial Intelligence, usually a Large Language Model (LLM). Additionally, it stores the conversation history.

## Setup
    
### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Plugins.Chat](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Chat) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Plugins.Chat

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.SemanticKernel.Plugins.Chat](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Chat) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Plugins.Chat

## How to use

To use [ChatWithHistoryPlugin](/Plugins/ChatWithHistoryPlugin.cs), the usual approach is to import it as a plugin within Semantic Kernel. The simplest way to do this is by using the extension method [ImportChatWithHistoryPluginUsingCosmosDb](/KernelExtensions.cs), which handles the import of the Plugin into Semantic Kernel. However, some previous configuration is required before importing it. 
First, you need to add the [SemanticKernelOptions](../Encamina.Enmarcha.SemanticKernel.Abstractions/SemanticKernelOptions.cs) and [ChatWithHistoryPluginOptions](./Plugins/ChatWithHistoryPluginOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings should look like using the `appsettings.json` file:

```json
  {
    // ...
    "SemanticKernelOptions": {
        "ChatModelName": "gpt-35-turbo", // Name (sort of a unique identifier) of the model to use for chat
        "ChatModelDeploymentName": "gpt-35-turbo", // Model deployment name on the LLM (for example OpenAI) to use for chat
        "EmbeddingsModelName": "text-embedding-ada-002", // Name (sort of a unique identifier) of the model to use for embeddings
        "EmbeddingsModelDeploymentName": "text-embedding-ada-002", // Model deployment name on the LLM (for example OpenAI) to use for embeddings
        "Endpoint": "https://your-url.openai.azure.com/", // Uri for an LLM resource (like OpenAI). This should include protocol and hostname.
        "Key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", // Key credential used to authenticate to an LLM resource
    },
    "ChatWithHistoryPluginOptions": {
        "HistoryMaxMessages": "gpt-35-turbo", // Name (sort of a unique identifier) of the model to use for chat
        "ChatRequestSettings": {
            "MaxTokens": 1000, // Maximum number of tokens to generate in the completion
            "Temperature": 0.8, // Controls the randomness of the completion. The higher the temperature, the more random the completion
            "TopP": 0.5, // Controls the diversity of the completion. The higher the TopP, the more diverse the completion.
        }
    },
    // ...
  }
```

Next, in `Program.cs` or a similar entry point file in your project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Requires Encamina.Enmarcha.SemanticKernel.Abstractions nuget
builder.Services.AddOptions<SemanticKernelOptions>().Bind(builder.Configuration.GetSection(nameof(SemanticKernelOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<ChatWithHistoryPluginOptions>().Bind(builder.Configuration.GetSection(nameof(ChatWithHistoryPluginOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Requieres Encamina.Enmarcha.Data.Cosmos
builder.Services.AddCosmos(builder.Configuration);

builder.Services.AddScoped(sp =>
{
    var kernel = new KernelBuilder()
        .WithAzureChatCompletionService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        //.WithOpenAIChatCompletionService("<YOUR MODEL ID>", "<YOUR API KEY>", "<YOUR API KEY>")
        /// ...
        .Build();

    // ...

    string cosmosContainer = "cosmosDbContainer"; // You probably want to save this in the appsettings or similar

    kernel.ImportChatWithHistoryPluginUsingCosmosDb(sp, cosmosContainer, ILengthFunctions.LengthByTokenCount);

    return kernel;
});
```

Now you can inject the kernel via constructor, and the chat capabilities are already available.

```csharp
public class MyClass
{
    private readonly Kernel kernel;

    public MyClass(Kernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task TestChatAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.ChatWithHistoryPlugin.Functions.Chat.Parameters.Ask, "What is the weather like in Madrid?");
        contextVariables.Set(PluginsInfo.ChatWithHistoryPlugin.Functions.Chat.Parameters.UserId, "123456");
        contextVariables.Set(PluginsInfo.ChatWithHistoryPlugin.Functions.Chat.Parameters.UserName, "John Doe");
        contextVariables.Set(PluginsInfo.ChatWithHistoryPlugin.Functions.Chat.Parameters.Locale, "en");

        var functionChat = kernel.Func(PluginsInfo.ChatWithHistoryPlugin.Name, PluginsInfo.ChatWithHistoryPlugin.Functions.Chat.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionChat);
    }
}
```

### Advanced configurations

Take into consideration that the above code uses a Cosmos DB implementation as IAsyncRepository as an example. You can use other implementations.

If you want to disable chat history, simply configure the [HistoryMaxMessages de ChatWithHistoryPluginOptions](/Plugins/ChatWithHistoryPluginOptions.cs) with a value of 0.

You can also inherit from the ChatWithHistoryPlugin class and add the customizations you need.

```csharp
public class MyCustomChatWithHistoryPlugin : ChatWithHistoryPlugin
{
    public MyCustomChatWithHistoryPlugin(Kernel kernel, string chatModelName, Func<string, int> tokensLengthFunction, IAsyncRepository<ChatMessageHistoryRecord> chatMessagesHistoryRepository, IOptionsMonitor<ChatWithHistoryPluginOptions> options)
        : base(kernel, chatModelName, tokensLengthFunction, chatMessagesHistoryRepository, options)
    {
    }

    protected override string SystemPrompt => "You are a Virtual Assistant who only talks about the weather.";

    // There are more overridable methods/properties
}
```