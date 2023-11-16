# Bot Skills Question Answering

This project mainly contains abstractions related to data used by other ENMARCHA NuGet packages or necessary for creating Question Answering skills

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Bot.Skills.QuestionAnswering](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Bot.Skills.QuestionAnswering

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Bot.Skills.QuestionAnswering](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Bot.Skills.QuestionAnswering

## How to use

In addition to the abstractions (interfaces, abstract classes, etc.) that have their implementations in other ENMARCHA NuGets
in Program.cs

```csharp
// Add services to the container.
builder.Services.AddQuestionAnsweringSkill(options =>
{
    options.QuestionAnsweringServiceName = "YourServiceName";
    options.DialogName = "YourDialogName";
    options.DialogIntent = "YourDialogIntent";
    options.NormalizeMessage = true;
    options.NormalizeRemoveCharacters = new List<char> { 'a', 'b', 'c' };
}, ServiceLifetime.Singleton);

```

## IServiceCollectionExtensions

`IServiceCollectionExtensions` is a static class that provides extension methods for `IServiceCollection` to add and configure services for a question answering skill and Cosmos DB support.

### Methods

- `AddQuestionAnsweringSkill`: Adds and configures the question answering dialog with given configuration options and question answering service name. It takes an `IServiceCollection`, an `Action<QuestionAnsweringSkillOptions>`, and an optional `ServiceLifetime` as parameters. It returns the `IServiceCollection` for chaining additional calls.

- `AddCosmosDbSupport`: Adds support for Cosmos DB with configuration parameters from the current configuration. It takes an `IServiceCollection`, an `IConfiguration`, and an optional `ServiceLifetime` as parameters. It returns the `IServiceCollection` for chaining additional calls.

## QuestionAnsweringDialog

`QuestionAnsweringDialog` is a class that defines a dialog for a question answering skill. It inherits from `NamedDialogBase` and implements the `IIntendable` interface.

### Usage

On Program.cs

```csharp

// Add bot dialogs...
builder.Services.AddSingleton<QuestionAnsweringDialog>().AddSingleton<Dialog, QuestionAnsweringDialog>();

```

### Constructor

- `QuestionAnsweringDialog`: Initializes a new instance of the class. It takes a string `id`, a `QuestionAnsweringDialogServices` object, and an `IOptionsMonitor<QuestionAnsweringSkillOptions>` as parameters.

### Fields

- `configurationOptions`: Stores the configuration options for this question answering dialog.

- `services`: Stores the services required for this question answering dialog.

- `isMetadataProcessorAvailable` and `isSourcesProcessorAvailable`: Indicate the availability of metadata and sources processors.

## QuestionAnsweringSkillOptions

`QuestionAnsweringSkillOptions` is a class that provides configuration options for the question answering dialog.

### Usage

To use this class, you need to create an instance and set the necessary properties:

```csharp
var options = new QuestionAnsweringSkillOptions
{
    QuestionAnsweringServiceName = "YourServiceName",
    DialogName = "YourDialogName",
    DialogIntent = "YourDialogIntent",
    NormalizeMessage = true,
    NormalizeRemoveCharacters = new List<char> { 'a', 'b', 'c' }
};
```

### Constructor

- `QuestionAnsweringSkillOptions`: Initializes a new instance of the class. It doesn't take any parameters.

### Properties

- `QuestionAnsweringServiceName`: Gets or sets the name of the Question Answering service to use by this skill.

- `DialogName`: Gets or sets the dialog's name.

- `DialogIntent`: Gets or sets the dialog's intent.

- `NormalizeMessage`: Gets or sets a value indicating whether the question answering dialog must normalize the message (question) removing specific characters (usually diacritics). Defaults to `false`.

- `NormalizeRemoveCharacters`: Gets or sets a collection of characters to remove from the message when normalizing.
