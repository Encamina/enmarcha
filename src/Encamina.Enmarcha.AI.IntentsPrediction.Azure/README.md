# AI - Azure Intent Prediction

Azure Intent Prediction is a wrapper project for Azure Cognitive Language Services Conversations client library. Its main functionality is to simplify and abstract the usage of the library, primarily focused on intention detection.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.IntentsPrediction.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.IntentsPrediction.Azure) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.IntentsPrediction.Azure

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.IntentsPrediction.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.IntentsPrediction.Azure) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.IntentsPrediction.Azure

## How to use

First, you need to add the [IntentPredictionConfigurations](./IntentPredictionConfigurations.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The following code is an example of how the settings should be set up using the appsettings.json file:

```json
  {
    // ...
    "IntentPredictionConfigurations": {    
      "IntentPredictionOptions": [
        {
          "Name": "DefaultIntentPrediction", // Name of this configuration
          "EndpointUrl": "https://example.cognitiveservices.azure.com/", // Language Service endpoint's url
          "KeyCredential": "<API-KEY>", // Language Service (security) key
          "DeploymentSlot": "development", // Deployment type, allowing the 'test' and 'prod' ('production' works as well)
          "OrchestratorName": "<ORCHESTRATOR-NAME>", // The name of the Orchestrator to use as intent prediction service from Azure
          "EnableLogging": true, // A flag to indicate whether logging is enabled
          "EnableVerbose": false, // A flag to indicate whether verbose mode is enabled
          "ConfidenceThreshold": 0.65 // Minimum threshold score for answers, value ranges from 0 to 1
        }
      ]
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

builder.Services.AddDefaultCognitiveServiceProvider()
                .AddAzureIntentPredictionServices(builder.Configuration);
```

The extension methods `AddDefaultCognitiveServiceProvider` and `AddAzureIntentPredictionServices` manage the configuration to create instances of `ICognitiveServiceProvider`. With this, you can retrieve instances of `IIntentPredictionService` (whose implementation is [IntentPredictionService](./IntentPredictionService.cs)). As seen in the `appsettings.json`, `IntentPredictionConfigurations` is an array, which allows you to generate different Intent Prediction configurations and retrieve information based on the `Name` parameter. You can inject `ICognitiveServiceProvider` through the constructor.

```csharp
public class MyClass
{
    private readonly IIntentPredictionService intentPredictionService;

    public MyClass(ICognitiveServiceProvider cognitiveServiceProvider)
    {
        // The value "DefaultIntentPrediction" is the name specified in the JSON from the previous code.
        // This is just an example code; avoid hardcoding strings :)
        intentPredictionService = cognitiveServiceProvider.GetIntentPredictionService("DefaultIntentPrediction");
    }

    public async Task<string> GetIntentAsync(string userInput, CancellationToken cancellationToken)
    {
        var intentPredictionRequest = new IntentPredictionRequest()
        {
            IntentPredictionOptions = new IntentPredictionOptions() { Language = CultureInfo.CurrentCulture },
            Utterance = userInput
        };

        var intentPredictionResult = await intentPredictionService.PredictIntentAsync(intentPredictionRequest, cancellationToken);

        return intentPredictionResult.IntentPrediction?.TopIntentName;
    }
}
```