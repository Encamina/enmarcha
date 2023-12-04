# AI - Azure Languages Detection

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AI.LanguagesDetection.Azure)](https://www.nuget.org/packages/Encamina.Enmarcha.AI.LanguagesDetection.Azure)

Azure Languages Detection is a wrapper project for Azure Cognitive Services Text Analytic client library and Azure AI Translator API REST. Its main functionality is to simplify and abstract the usage of the library, primarily focused on detect language.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.LanguagesDetection.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.LanguagesDetection.Azure) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.LanguagesDetection.Azure

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AI.LanguagesDetection.Azure](https://www.nuget.org/packages/Encamina.Enmarcha.AI.LanguagesDetection.Azure) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.LanguagesDetection.Azure

## How to use
There are two implementations for [ILanguageDetectionService](../Encamina.Enmarcha.AI.LanguagesDetection.Abstractions/ILanguageDetectionService.cs), one utilizing the Azure Cognitive Services Text Analytic client library, and the other using the Azure AI Translator API REST. 

### Azure Text Analytics

First, you need to add the [TextAnalyticsLanguageDetectionConfigurations](./TextAnalyticsLanguageDetectionConfigurations.cs) data to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "TextAnalyticsLanguageDetectionConfigurations": {
        "TextAnalyticsLanguageDetectionServiceOptions": [
          {
            "Name": "DefaultAnalyticsLanguageDetection", // Name of this configuration
            "EndpointUrl": "https://example.cognitiveservices.azure.com/", // Text Analytic endpoint's url
            "KeyCredential": "<API-KEY>", // Text Analytic (security) key
            "ConfidenceThreshold": 0.65, // Minimum threshold score for text analytic, value ranges from 0 to 1
            "IncludeStatistics": true // A flag to indicate whether if the service should return statistics with the results of the operation
        /// results of the operation
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
                .AddAzureTextAnalyticsLanguageDetectionServices(builder.Configuration);
```

The extension methods `AddDefaultCognitiveServiceProvider` and `AddAzureTextAnalyticsLanguageDetectionServices` manage the configuration to create instances of `ICognitiveServiceProvider`. With this, you can retrieve instances of `ILanguageDetectionService` (whose implementation is [TextAnalyticsLanguageDetectionService](./TextAnalyticsLanguageDetectionService.cs)). As seen in the configuration JSON, `TextAnalyticsLanguageDetectionConfigurations` is an array, allowing you to generate different Text Analytics Language Detection configurations and retrieve information based on the `Name` value. Now, you can inject `ICognitiveServiceProvider` through the constructor for use.

```csharp
public class MyClass
{
    private readonly ILanguageDetectionService languageDetectionService;

    public MyClass(ICognitiveServiceProvider cognitiveServiceProvider)
    {
        // The value "DefaultAnalyticsLanguageDetection" is the name specified in the JSON from the previous code.
        // This is just an example code; avoid hardcoding strings :)
        languageDetectionService = cognitiveServiceProvider.GetLanguageDetectionService("DefaultAnalyticsLanguageDetection");
    }

    public async Task<CultureInfo> GetLanguageAsync(string userInput, CancellationToken cancellationToken)
    {
        var languageDetectionRequest = new LanguageDetectionRequest() { Text = new[] { new Text(value: userInput) } };

        var languageDetectionResult = await languageDetectionService.DetectLanguageAsync(languageDetectionRequest, CancellationToken.None);

        return languageDetectionResult.DetectedLanguages.MaxBy(d => d.ConfidenceScore).Language;
    }
}
```

### Azure Translator

First, you need to add the [TranslatorLanguageDetectionConfigurations](./TranslatorLanguageDetectionConfigurations.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "TranslatorLanguageDetectionConfigurations": {
      "TranslatorLanguageDetectionService": [
        {
          "Name": "DefaultAnalyticsLanguageDetection", // Name of this configuration
          "EndpointUrl": "https://example.cognitiveservices.azure.com/", // Translator endpoint's url
          "KeyCredential": "<API-KEY>", // Translator Analytic (security) key
          "ConfidenceThreshold": 0.65, // Minimum threshold score for language detection, value ranges from 0 to 1
          "DetectOnlyTranslatableLanguages": true, // A flag to indicate whether any language detection result should only include languages that can be translated
          "RegionName": "westus" // Azure region name of the translator resource.
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
                .AddAzureTranslatorLanguageDetectionServices(builder.Configuration);
```
The extension methods `AddDefaultCognitiveServiceProvider` and `AddAzureTranslatorLanguageDetectionServices` manage the configuration to create instances of `ICognitiveServiceProvider`. With this, you can retrieve instances of `ILanguageDetectionService` (whose implementation is [TranslatorLanguageDetectionService](./TranslatorLanguageDetectionService.cs)). As seen in the configuration JSON, `TranslatorLanguageDetectionConfigurations` is an array, allowing you to generate different Translator Language Detection configurations and retrieve the appropriate one based on the `Name`. Now, you can inject `ICognitiveServiceProvider` through the constructor for use.

```csharp
public class MyClass
{
    private readonly ILanguageDetectionService languageDetectionService;

    public MyClass(ILanguageDetectionService languageDetectionService)
    {
        // The value "DefaultAnalyticsLanguageDetection" is the name specified in the JSON from the previous code.
        // This is just an example code; avoid hardcoding strings :)
        languageDetectionService = cognitiveServiceProvider.GetLanguageDetectionService("DefaultAnalyticsLanguageDetection");
    }

    public async Task<CultureInfo> GetLanguageAsync(string userInput, CancellationToken cancellationToken)
    {
        var languageDetectionRequest = new LanguageDetectionRequest() { Text = new[] { new Text(value: userInput) } };

        var languageDetectionResult = await languageDetectionService.DetectLanguageAsync(languageDetectionRequest, CancellationToken.None);

        return languageDetectionResult.DetectedLanguages.MaxBy(d => d.ConfidenceScore).Language;
    }
}
```