# AI - Azure Texts Translation

Azure Texts Translation is a wrapper project for Azure AI Translator API REST. Its main functionality is to simplify and abstract the usage of API REST, primarily focused on text translations.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.TextsTranslation.Azure](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.TextsTranslation.Azure

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AI.TextsTranslation.Azure](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.TextsTranslation.Azure

## How to use
### Question Answering

First, you need to add the [TextTranslationConfigurations](./TextTranslationConfigurations.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "TextTranslationConfigurations": {
        "TextTranslationOptions": [
        {
            "Name": "DefaultTextTranslation", // Name of this configuration
            "EndpointUrl": "https://example.cognitiveservices.azure.com/", // Azure AI Translator endpoint's url
            "KeyCredential": "<API-KEY>", // Azure AI Translator (security) key
            "RegionName": "westeurope" // Azure region name of the translator resource.
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
       .AddAzureTextTranslationServices(builder.Configuration);
```
The extension methods `AddDefaultCognitiveServiceProvider` and `AddAzureTextTranslationServices` manage the configuration to create instances of `ICognitiveServiceProvider`. With this, you can retrieve instances of `ITextTranslationService` (whose implementation is [TextTranslationService](./TextTranslationService.cs)). As seen in the configuration JSON, `TextTranslationConfigurations` is an array, allowing you to generate different Text Translations configurations and retrieve the appropriate one based on the `Name`. Now, you can inject `ICognitiveServiceProvider` through the constructor for use.

```csharp
public class MyClass
{
    private readonly ITextTranslationService textTranslationService;

    public MyClass(ICognitiveServiceProvider cognitiveServiceProvider)
    {
        // The value "DefaultTextTranslation" is the name specified in the JSON from the previous code.
        // This is just an example code; avoid hardcoding strings :)
        textTranslationService = cognitiveServiceProvider.GetTextTranslationService("DefaultTextTranslation");
    }

    public async Task<string> GetEnglishTranslationAsync(string text, CancellationToken cancellationToken)
    {
        const string languageName = "en-ES";
        const string key = "text_key";

        var textTranslationRequest = new TextTranslationRequest()
        {
            ToLanguages = new CultureInfo[] { new CultureInfo(languageName) },
            Texts = new Dictionary<string, string>() { { key, text } },
        };

        var textTranslationResult = await textTranslationService.TranslateAsync(textTranslationRequest, cancellationToken);

        var translations = textTranslationResult.TextTranslations?.FirstOrDefault(t => t.Id == key)?.Translations;

        return translations != null && translations.TryGetValue(languageName, out var englishTranslation)
            ? englishTranslation
            : null;
    }
}
```

In addition, you can intercept the translated texts to apply fixes or changes to overcome or correct any discrepancy. To do this, simply implement the `ITextTranslationNormalizer` interface.

```csharp
public class NumberNormalizer : ITextTranslationNormalizer
{
    private static readonly Dictionary<string, string> numbers = new Dictionary<string, string>
    {
        {"0", "Zero"},
        {"1", "One"},
        {"2", "Two"},
        {"3", "Three"},
        {"4", "Four"},
        {"5", "Five"},
        {"6", "Six"},
        {"7", "Seven"},
        {"8", "Eight"},
        {"9", "Nine"}
    };

    public int Order => int.MinValue;

    public string Normalize(string value)
    {
        foreach (var number in numbers)
        {
            value = value.Replace(number.Key, number.Value, StringComparison.InvariantCulture);
        }

        return value;
    }
}

public class TrimNormalizer : ITextTranslationNormalizer
{
    public int Order => int.MaxValue;

    public string Normalize(string value)
    {
        return value.Trim();
    }
}
```

`NumberNormalizer` replaces any occurrence of a number (1, 2, 3...) with its textual representation (one, two, three...), and `TrimNormalizer` simply removes leading and trailing spaces from the text. As you can see, the value of the `Order` property indicates that `NumberNormalizer` will be applied first, followed by `TrimNormalizer`. In the initial code, it is sufficient to add the created _normalizers_.

```csharp
// Entry point

// ...

builder.Services.AddDefaultCognitiveServiceProvider()
       .AddAzureTextTranslationServices(builder.Configuration)
       .UseNormalizer<NumberNormalizer>()
       .UseNormalizer<TrimNormalizer>();
```
Now, when you execute the code for the previous translation (the `MyClass` class), it will automatically utilize the _normalizers_ on the output text.