# AI - Text Translations Abstractions

This project mainly contains Text Translations abstractions used by other ENMARCHA NuGet packages. These abstractions include interfaces that have their implementations in other projects, and entities that represent abstractions for information exchange. The NuGet package `Encamina.Enmarcha.AI.TextsTranslation.Azure` contains implementations of this package (see [documentation](../Encamina.Enmarcha.AI.TextsTranslation.Azure/README.md)).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.TextsTranslation.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.TextsTranslation.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.TextsTranslation.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.TextsTranslation.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.TextsTranslation.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.TextsTranslation.Abstractions

## How to use

The primary way to leverage this project is by applying its interfaces within other projects. The main interfaces are the following:
- [ITextTranslationService](./ITextTranslationService.cs) is an interface that represents a cognitive service that provides text translation capabilities. Its implementation is available in the NuGet package (see [documentation](../Encamina.Enmarcha.AI.TextsTranslation.Azure/README.md)).
- [ITextTranslationNormalizer](./ITextTranslationNormalizer.cs) is an interface that represents normalizer that fixes or changes results from text translations to overcome or correct any discrepancy.

There are other classes like `TextTranslationRequest`, `TextTranslationResult` and more, which are entities for information exchange.