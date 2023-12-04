# AI

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AI)](https://www.nuget.org/packages/Encamina.Enmarcha.AI)

This project contains base classes, interfaces, and common functionalities shared across all ENMARCHA projects related to AI. Essentially, it serves as the common ground for orchestrating the creation and retrieval of cognitive services.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI](https://www.nuget.org/packages/Encamina.Enmarcha.AI) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI](https://www.nuget.org/packages/Encamina.Enmarcha.AI) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI

## How to use
The main way to use this project is in conjunction with another ENMARCHA project related to AI. The following code snippet represents the most common usage of this project. Starting from a `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services.AddDefaultCognitiveServiceProvider();
```

The `AddDefaultCognitiveServiceProvider` extension method adds a default cognitive service provider to the `IServiceCollection` as a singleton. This allows you to resolve the [ICognitiveServiceProvider](./ICognitiveServiceProvider.cs) interface, which is responsible for providing instances of cognitive services based on their name. This setup enables you to configure different implementations for the same cognitive service and retrieve them based on their respective names.

```csharp
public class MyClass
{
    private readonly ICognitiveServiceProvider cognitiveServiceProvider;

    public MyClass(ICognitiveServiceProvider cognitiveServiceProvider)
    {
        this.cognitiveServiceProvider = cognitiveServiceProvider;
    }

    public Task PredictIntentAsync()
    {
        // The name is configured when setting up the Intent Prediction service. This service is not
        // included within this NuGet package.
        var intentPredictionService = cognitiveServiceProvider.GetIntentPredictionService("NAME_OF_INTENT_PREDICTION_SERVICE");

        // Utilizing the Intent Prediction Service...

        return Task.CompletedTask;
    }
}
```
The above code is not fully functional as the intent prediction service (available in another NuGet package) has yet to be configured.

Another functionality available in this NuGet package is an implementation of [ITextSplitter](../Encamina.Enmarcha.AI.Abstractions/ITextSplitter.cs), Specifically [RecursiveCharacterTextSplitter](./TextSplitters/RecursiveCharacterTextSplitter.cs). This implementation is the recommended of `ITextSplitter` for generic texts. It splits texts in order until the chunks are small enough (based on ITextSplitter.ChunkSize). As long as possible, it will strive to keep all paragraphs, sentences, and then words, intact, since those would generically seem to be the strongest semantically related pieces of text that could be split. There is an extension method available to add it directly to the dependency container. First, you need to add the [TextSplitterOptions](../Encamina.Enmarcha.AI.Abstractions/TextSplitterOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The following code is an example of the `appsettings.json` file that contains the TextSplitterOptions settings:

```json
{
    // ...
    "TextSplitterOptions": {
        "ChunkOverlap": 64, // Number of elements (characters, tokens, etc.) overlapping between chunks
        "ChunkSize": 512, // Number of elements (characters, tokens, etc.) in each chunk.
        "Separators": [ "\n\r", "\n\n", "\n", ". ", ", ", " ", "" ] // Collection of separator characters to use when splitting the text and creating chunks
    },
    // ...
}
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

builder.Services.AddOptions<TextSplitterOptions>().Bind(builder.Configuration.GetSection(nameof(TextSplitterOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddRecursiveCharacterTextSplitter();
```

The extension method `AddRecursiveCharacterTextSplitter` manages to configure everything necessary to create instances of `ITextSplitter`. With this setup, you can retrieve instances of `ITextSplitter` (whose implementation is [RecursiveCharacterTextSplitter](./TextSplitters/RecursiveCharacterTextSplitter.cs)). Now, you can resolve `ITextSplitter` interface as needed.

```csharp
public class MyClass
{
    private readonly ITextSplitter textSplitter;

    public MyClass(ITextSplitter textSplitter)
    {
        this.textSplitter = textSplitter;
    }

    public IEnumerable<string> SplitText(string text)
    {        
        return textSplitter.Split(text, lengthFunction: ILengthFunctions.LengthByCharacterCount);
    }
}
```
