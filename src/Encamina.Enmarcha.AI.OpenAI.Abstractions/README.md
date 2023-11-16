# AI - OpenAI Abstractions

This project mainly contains abstractions used by other ENMARCHA NuGet packages. These abstractions include interfaces that have their implementations in other projects, and entities that represent abstractions for information exchange. It also contains some functionality with OpenAI. The NuGet package `Encamina.Enmarcha.AI.OpenAI.Azure` contains implementations of this package (see [documentation](../Encamina.Enmarcha.AI.OpenAI.Azure/README.md)).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.OpenAI.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.OpenAI.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.OpenAI.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.OpenAI.Abstractions

## How to use

The primary way of using this project is through its interfaces, using some of their implementations generated in Encamina.Enmarcha.AI.OpenAI.Azure NuGet package. The main interface is the following:
- [ICompletionService](./ICompletionService.cs) is an interface that represents an OpenAI completion service which from some input text as a prompt, will generate a text completion that attempts to match whatever context or pattern has been given to an under laying model. It has its implementation in Encamina.Enmarcha.AI.OpenAI.Azure NuGet (see [documentation](../Encamina.Enmarcha.AI.OpenAI.Azure/README.md)).

It also contains the class [ModelInfo](./ModelInfo.cs) that provides information about an OpenAI model.

```csharp
var gpt35TurboMaxTokens = ModelInfo.GetById("gpt-35-turbo").MaxTokens;
// gpt35TurboMaxTokens => 4096
```

It has an extension method to adds a default provider for factories of completion services based on OpenAI.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services.AddDefaultCompletionServiceFactoryProvider();
```
This extension method will add the default implementation of the [ICompletionServiceFactoryProvider](./ICompletionServiceFactoryProvider.cs) interface as a singleton. The default implementation is [CompletionServiceFactoryProvider](./Internals/CompletionServiceFactoryProvider.cs). With this, we can resolve the `ICompletionServiceFactoryProvider` interface.