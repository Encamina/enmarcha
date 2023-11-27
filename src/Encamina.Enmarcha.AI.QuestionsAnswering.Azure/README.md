# AI - Azure Questions Answering

Azure Intent Prediction is a wrapper project for Azure Cognitive Language Services Question Answering client library. Its main functionality is to simplify and abstract the usage of the library, primarily focused on question answering.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.QuestionsAnswering.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.QuestionsAnswering.Azure) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.QuestionsAnswering.Azure

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.QuestionsAnswering.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.QuestionsAnswering.Azure) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.QuestionsAnswering.Azure

## How to use

### Question Answering

First, you need to add the [QuestionAnsweringConfigurations](./QuestionAnsweringConfigurations.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "QuestionAnsweringConfigurations": {
      "QuestionAnsweringOptions": [
        {
          "Name": "DefaultQuestionAnswering", // Name of this configuration
          "EndpointUrl": "https://example.cognitiveservices.azure.com/", // Language Service endpoint's url
          "KeyCredential": "<API-KEY>", // Language Service (security) key
          "DeploymentSlot": "development", // Deployment type, allowing the 'test' and 'prod' ('production' works as well)
          "KnowledgeBaseName": "<YOUR-PROJECT-NAME>", // Name of the Question Answering project in Language Service which represents a Knowledge Base
          "ConfidenceThreshold": 0.65, // Minimum threshold score for answers, value ranges from 0 to 1
          "Top": 3 // Maximum number of answers to be returned per question.
        }
      ]
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
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

builder.Services.AddDefaultCognitiveServiceProvider()
                .AddAzureQuestionAnsweringServices(builder.Configuration);
```
The extension methods `AddDefaultCognitiveServiceProvider` and `AddAzureQuestionAnsweringServices` manage the configuration to create instances of `ICognitiveServiceProvider`. With this, you can retrieve instances of `IQuestionAnsweringService` (whose implementation is [QuestionAnsweringService](./QuestionAnsweringService.cs)). As seen in the configuration JSON, `QuestionAnsweringConfigurations` is an array, allowing you to generate different Question Answerings configurations and retrieve the appropriate one based on the `Name`. Now, you can inject `ICognitiveServiceProvider` through the constructor for use.

```csharp
public class MyClass
{
    private readonly IQuestionAnsweringService questionAnsweringService;

    public MyClass(ICognitiveServiceProvider cognitiveServiceProvider)
    {
        // The value "DefaultQuestionAnswering" is the name specified in the JSON from the previous code.
        // This is just an example code; avoid hardcoding strings :)
        questionAnsweringService = cognitiveServiceProvider.GetQuestionsAnsweringService("DefaultQuestionAnswering");
    }

    public async Task<string> GetAnswerAsync(string userInput, CancellationToken cancellationToken)
    {
        var questionRequest = new QuestionRequest() { Question = userInput };

        var questionResult = await questionAnsweringService.GetAnswersAsync(questionRequest, CancellationToken.None);

        return questionResult.Answers.MaxBy(d => d.ConfidenceScore).Value;
    }
}
```

### Metadata Handler

[CachedTableStorageCompositeMetadataHandler](./Metadata/CachedTableStorageCompositeMetadataHandler.cs) provides metadata handling using parameters stored in an Azure Table Storage with the optional posibility to cached these parameters to improve performance. First, you need to add the Azure Storage data to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the `appsettings.json` file:

```json
{
  // ...
  "ConnectionStrings": {
    "TableStorage": "<TABLE-STORAGE-CONNECTION-STRING>" // Table Storage connection string
  },
  "TableName": "Metadata", // Table Storage table name
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

builder.Services.AddCachedTableStorageCompositeMetadataHandler(options =>
{
    options.TableConnectionString = builder.Configuration.GetConnectionString("TableStorage");
    options.TableName = builder.Configuration.GetValue<string>("TableName");
});
```

The extension methods `AddCachedTableStorageCompositeMetadataHandler` manages of configuring everything necessary to create instances of `IMetadataHandler` based on [CachedTableStorageCompositeMetadataHandler](./Metadata/CachedTableStorageCompositeMetadataHandler.cs) implementation. Now, you can inject `IMetadataHandler` through the constructor for use.

```csharp
public class MyClass
{
    private readonly IMetadataHandler metadataHandler;

    public MyClass(IMetadataHandler metadataHandler)
    {
        this.metadataHandler = metadataHandler;
    }

    public async Task<IDictionary<string, string>> GetMetadataAsync(string message, CancellationToken cancellationToken)
    {
        var metadataOptions = await metadataHandler.HandleMessageAsync(message, currentMetadataOptions: null, cancellationToken);

        return metadataOptions.Metadata;
    }
}
```