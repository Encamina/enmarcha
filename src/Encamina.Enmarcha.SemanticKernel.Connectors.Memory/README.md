# Semantic Kernel - Memory Connectors

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.SemanticKernel.Connectors.Memory)](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Connectors.Memory)

Memory Connectors is a project that allows adding specific [IMemoryStore](https://github.com/microsoft/semantic-kernel/blob/76db027273371ea81e6db66afcb1d888cc53b459/dotnet/src/SemanticKernel.Abstractions/Memory/IMemoryStore.cs#L13) instances. These `IMemoryStore` instances are used for storing and retrieving embeddings.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Connectors.Memory](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Connectors.Memory) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Connectors.Memory

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.SemanticKernel.Connectors.Memory](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Connectors.Memory) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Connectors.Memory

## How to use

First, you need to add the [QdrantOptions](../Encamina.Enmarcha.Data.Qdrant.Abstractions/QdrantOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the `appsettings.json` file:

```json
// ...
  "QdrantOptions": {
    "Host": "https://sample-qdrant.azurewebsites.net/", // Endpoint protocol and host
    "Port": 6333, // Endpoint port
    "VectorSize": 1536, // Vector size
    "ApiKey": "xxxxxxxxxx" // API Key used by Qdrant as a form of client authentication.
  },
// ...
```

Next, in `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

builder.Services.AddOptions<QdrantOptions>().Bind(builder.Configuration.GetSection(nameof(QdrantOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Adds Qdrant as IMemoryStore
services.AddQdrantMemoryStore();
```

In the previous code, it can be observed that in the first part is necessary to add certain [Qdrant configurations](../Encamina.Enmarcha.Data.Qdrant.Abstractions/QdrantOptions.cs) that are available in the [Encamina.Enmarcha.Data.Qdrant.Abstractions](../Encamina.Enmarcha.Data.Qdrant.Abstractions/README.md) nuget package. The last line of code corresponds to an extension method that will add the specified implementation of the [IMemoryStore](https://github.com/microsoft/semantic-kernel/blob/76db027273371ea81e6db66afcb1d888cc53b459/dotnet/src/SemanticKernel.Abstractions/Memory/IMemoryStore.cs#L13) interface as a singleton. With this, you have Qdrant configured as the storage to save and retrieve embeddings.

After the initial configuration, we typically configure Semantic Kernel as `Scoped`.

```csharp
builder.Services.AddScoped(sp =>
{
    var kernel = new KernelBuilder()
        .WithAzureTextEmbeddingGenerationService("<YOUR DEPLOYMENT NAME>", "<YOUR AZURE ENDPOINT>", "<YOUR API KEY>")
        //.WithOpenAITextEmbeddingGenerationService("<YOUR MODEL ID>", "<YOUR API KEY>", "<YOUR API KEY>")
        /// ...
        .Build();

    // ...

    return kernel;
});
```

Once configured, you can now use Semantic Kernel, and it will utilize the Qdrant storage we have previously set up (in addition to generating the embeddings).

```csharp
public class MyClass
{
    private readonly Kernel kernel;

    public MyClass(Kernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task MyTestMethodAsync()
    {
        await kernel.Memory.SaveInformationAsync("my-collection", "my dummy text", Guid.NewGuid().ToString());
        var memoryQueryResult = await kernel.Memory.SearchAsync("my-collection", "my similar dummy text")
            .ToListAsync(); // ToListAsync method is provided by System.Linq.Async nuget https://www.nuget.org/packages/System.Linq.Async
    }
}
```

If you prefer, you can inject the `ISemanticTextMemory` interface directly.

```csharp
public class MyClass
{   
    private readonly ISemanticTextMemory semanticTextMemory;

    public MyClass(ISemanticTextMemory semanticTextMemory)
    {
        this.semanticTextMemory = semanticTextMemory;
    }

    public async Task MyTestMethodAsync()
    {
        await semanticTextMemory.SaveInformationAsync("my-collection", "my dummy text", Guid.NewGuid().ToString());
        var memoryQueryResult = await semanticTextMemory.SearchAsync("my-collection", "my similar dummy text")
            .ToListAsync(); // ToListAsync method is provided by System.Linq.Async nuget https://www.nuget.org/packages/System.Linq.Async
    }
}
```