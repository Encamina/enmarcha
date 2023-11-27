# Semantic Kernel - Memory Plugin

Memory Plugin is a project that offers functionality as a plugin for searching the memory of the Semantic Kernel. When text is stored in memory within the Semantic Kernel, it is saved in the form of embeddings that can be queried through the Memory Plugin.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Plugins.Memory](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Memory) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Plugins.Memory

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.SemanticKernel.Plugins.Memory](https://www.nuget.org/packages/Encamina.Enmarcha.SemanticKernel.Plugins.Memory) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Plugins.Memory

## How to use

To use [MemoryQueryPlugin](/Plugins/MemoryQueryPlugin.cs), the usual approach is to import it as a plugin within Semantic Kernel. The simplest way to do this is by using the extension method [ImportMemoryPlugin](/IKernelExtensions.cs), which handles the import of the Plugin into Semantic Kernel. 

```csharp
// Entry point
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
        /// ...
        .Build();

    // ...

    kernel.ImportMemoryPlugin(ILengthFunctions.LengthByTokenCount);

    return kernel;
});
```

Now you can inject the kernel via constructor, and the memory capabilities are already available.

```csharp
public class MyClass
{
    private readonly IKernel kernel;

    public MyClass(IKernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task TestMemoryQueryAsync()
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.Query, "What is the weather like in Madrid?");
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.CollectionsStr, "collection-1:collection-2");
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.CollectionSeparator, ":");
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.MinRelevance, "0.8");
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.ResponseTokenLimit, "1500");
        contextVariables.Set(PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Parameters.ResultsLimit, "2");

        var functionMemoryQuery = kernel.Func(PluginsInfo.MemoryQueryPlugin.Name, PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Name);

        var resultContext = await kernel.RunAsync(contextVariables, functionMemoryQuery);
    }
}
```
Within `resultContext`, you will find texts that meet the conditions.