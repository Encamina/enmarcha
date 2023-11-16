# AI - Azure OpenAI

This project focuses on making it easier to use Azure OpenAI services to utilize the completion service.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.OpenAI.Azure](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.OpenAI.Azure

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AI.OpenAI.Azure](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.OpenAI.Azure

## How to use

First, you need to add the [CompletionServiceOptions](./CompletionServiceOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "CompletionServiceOptions": {
        "DeploymentName": "<YOUR-COMPLETION-MODEL-NAME>", // Name of the deployed model to use with this completion service
        "Name": "completion-service-test", // Name of this completion service
        "EndpointUrl": "<YOUR-AZURE-OPEN-AI-ENDPOINT>", // /Azure OpenAI endpoint's url
        "KeyCredential": "<YOUR-AZURE-API-KEY>" // Azure OpenAI (security) key
    }
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
builder.Configuration.AddJsonFile(@"appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddAzureOpenAICompletionService(builder.Configuration);
```
The `AddAzureOpenAICompletionService` extension method will add the default implementation of the [ICompletionService](../Encamina.Enmarcha.AI.OpenAI.Abstractions/ICompletionService.cs) interface as `Transient`. The default implementation is [CompletionService](./CompletionService.cs). With this, we can resolve the `ICompletionService` with construction injection. 

```csharp
public class MyClass
{
    private readonly ICompletionService completionService;

    public MyClass(ICompletionService completionService)
    {
        this.completionService = completionService;
    }

    public async Task TestCompletionAsync()
    {
        var completionResult = await completionService.CompleteAsync(new CompletionRequest()
        {
            DoEcho = null,
            BestOf = null,
            Prompts = new List<string> { "Console.WriteL" },
            StopSequences = Enumerable.Empty<string>(),
            NumberOfCompletionsPerPrompt = 1,
            Temperature = 0,
            MaxTokens = 20
        }, CancellationToken.None);

        var firstResult = completionResult.Completitions.First().Text;
    }
}
```
Within `firstResult`, you will find texts that say something like _Ine("Hello World");_.