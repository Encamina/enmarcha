# Semantic Kernel - Abstractions

This project mainly contains abstractions used by other ENMARCHA NuGet packages. Additionally, it includes extension methods and utilities primarily related to Semantic Kernel.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.SemanticKernel.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.SemanticKernel.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.SemanticKernel.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.SemanticKernel.Abstractions

## How to use

In addition to the abstractions such as interfaces or abstract classes, which have their implementations in other ENMARCHA NuGets (for example, [IMemoryManager](./IMemoryManager.cs)/[MemoryManager](../Encamina.Enmarcha.SemanticKernel/MemoryManager.cs)), there are some utilities that can be _used directly_.

### ILengthFunctions

```csharp
// Entry point
var tokenCount = ILengthFunctions.LengthByTokenCount("This text contains 5 tokens");
// tokenCount = 5
```

The above code returns the number of tokens in the text using the GPT-3 tokenizer.

### SemanticKernelOptions

```json
// ...
  "SemanticKernelOptions": {
    "ChatModelName": "gpt-35-turbo", // Name (sort of a unique identifier) of the model to use for chat
    "ChatModelDeploymentName": "gpt-35-turbo", // Model deployment name on the LLM (for example OpenAI) to use for chat
    "EmbeddingsModelName": "text-embedding-ada-002", // Name (sort of a unique identifier) of the model to use for embeddings
    "EmbeddingsModelDeploymentName": "text-embedding-ada-002", // Model deployment name on the LLM (for example OpenAI) to use for embeddings
    "Endpoint": "https://your-url.openai.azure.com/", // Uri for an LLM resource (like OpenAI). This should include protocol and hostname.
    "Key": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", // Key credential used to authenticate to an LLM resource
    // ....
  },
// ...
```

The above section is taken from a JSON file that is typically added to the `appsettings.json`. This provides the necessary configurations for Semantic Kernel to operate.

Once these configurations are defined, they can be added to the application's configuration.

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddOptions<SemanticKernelOptions>().Bind(builder.Configuration.GetSection(nameof(SemanticKernelOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### SKContextExtensions

This class provides some static methods to obtain information/functionality about `SKContext`.
```csharp
// ...

var resultContext = await kernel.RunAsync(contextVariables);

// Extensions method from SKContextExtensions
resultContext.ValidateAndThrowIfErrorOccurred();
var maxWordCount = resultContext.GetContextVariable("dummycontextvariable");
```