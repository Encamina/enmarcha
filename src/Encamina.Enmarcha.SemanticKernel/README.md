# Semantic Kernel

This project provides extended functionality for Semantic Kernel and additional features related to Semantic Kernel.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel

### .NET CLI:

Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.SemanticKernel](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel

## How to use

### MemoryManager

[MemoryManager](./MemoryManager.cs) provides some CRUD operations over memories with multiple chunks that need to be managed `IMemoryManager`, using batch operations. 
 
Starting from a `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ... 

builder.Services.AddScoped(sp =>
{
    var kernel = new KernelBuilder()
        .WithAzureTextEmbeddingGenerationService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        //.WithOpenAITextEmbeddingGenerationService("<YOUR MODEL ID>", "<YOUR API KEY>")
        // ...
        .Build();

    // ...

    return kernel;
});

// Here use the desired implementation (Qdrant, Volatile...)
builder.Services.AddSingleton<IMemoryStore, VolatileMemoryStore>();

builder.Services.AddMemoryManager();
```

This extension method will add the default implementation of the [IMemoryManager](../Encamina.Enmarcha.SemanticKernel.Abstractions/IMemoryManager.cs) interface as a singleton. The default implementation is [MemoryManager](./MemoryManager.cs). With this, we can resolve the `IMemoryManager` via constructor.

```csharp
public class MyClass
{
    private readonly IMemoryManager memoryManager;

    public MyClass(IMemoryManager memoryManager)
    {
        this.memoryManager = memoryManager;
    }

    public async Task TestMemoryManagerAsync()
    {
        await memoryManager.UpsertMemoryAsync("123456", "my-collection", new List<string>() { "First chunk", "Second chunk", "Third chunk" }, CancellationToken.None);
        var memoryContent = await memoryManager.GetMemoryAsync("123456", "my-collection", CancellationToken.None);

        // memoryContent.Chunks contains  "First chunk", "Second chunk", "Third chunk" chunks
    }
}
```
### EphemeralMemoryStoreHandler

[EphemeralMemoryStoreHandler](./EphemeralMemoryStoreHandler.cs) is a memory store handler that removes collections from memory after a configured time (thus ephemeral) of inactivity. First, you need to add the [EphemeralMemoryStoreHandlerOptions](./Options/EphemeralMemoryStoreHandlerOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "EphemeralMemoryStoreHandlerOptions": {    
        "IdleTimeoutMinutes": 60, // Idle time in minutes after which a memory is considered inactive and can be removed from the memory store.
        "InactivePollingTimeMinutes": 10, // Time in minutes to wait before polling for inactive memories
    },
    // ...
  }
```

Next, in `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ... 

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

builder.Services.AddOptions<EphemeralMemoryStoreHandlerOptions>().Bind(builder.Configuration.GetSection(nameof(EphemeralMemoryStoreHandlerOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Here use the desired implementation (Qdrant, Volatile...)
builder.Services.AddSingleton<IMemoryStore, VolatileMemoryStore>();

builder.Services.AddEphemeralMemoryStoreHandler(builder.Configuration);
```

This extension method will add [EphemeralMemoryStoreHandler](./EphemeralMemoryStoreHandler.cs.cs) as implementation of the [IMemoryStoreHandler](../Encamina.Enmarcha.SemanticKernel.Abstractions/IMemoryStoreHandler.cs) interface as a singleton. Now you can inject `IMemoryStoreHandler` via constructor.

```csharp
public class MyClass
{
    private readonly IMemoryStoreHandler memoryStoreHandler;

    public MyClass(IMemoryStoreHandler memoryStoreHandler)
    {
        this.memoryStoreHandler = memoryStoreHandler;
    }

    public async Task TestGetCollectionnameAsync()
    {
        var collectionName = await memoryStoreHandler.GetCollectionNameAsync("my-collection", CancellationToken.None);

    }
}
```

This allows the my-collection collection stored in memory to be tracked, and after [EphemeralMemoryStoreHandlerOptions.IdleTimeoutMinutes](./Options/EphemeralMemoryStoreHandlerOptions.cs) since the last access to the collection through the `GetCollectionNameAsync` method, it will be deleted.

### IKernelExtensions

Contains extension methods for IKernel. You can see all available extension methods in the [IKernelExtensions](./Extensions/IKernelExtensions.cs) class.

```csharp
    var mySemanticFunction = kernel.Skills.GetFunction("MyFunctionName");
    var myContextVariables = new ContextVariables();
    myContextVariables.Set(@"input", "Dummy input");

    //  Calculates the current total number of tokens used in generating a prompt of a mySemanticFunction from embedded resources in an assembly, using myContextVariables.
    var mySemanticFunctionUsedTokens = await kernel.GetSemanticFunctionUsedTokensAsync(mySemanticFunction, Assembly.GetExecutingAssembly(), myContextVariables, ILengthFunctions.LengthByTokenCount, CancellationToken.None);


    // ...

    // Imports plugins with semantic functions from embedded resources in an assembly that represents their prompt and configuration files.
    kernel.ImportSemanticPluginsFromAssembly(Assembly.GetExecutingAssembly());

    // More extensions methods availables...
```