# AI - Languages Detection Abstractions

This project mainly contains Languages Detection abstractions used by other ENMARCHA NuGet packages. This abstractions include interfaces, which have their implementations in other projects, and entities that represent abstractions for information exchange. The NuGet package `Encamina.Enmarcha.AI.LanguagesDetection.Azure` contains implementations of this package (see [documentation](../Encamina.Enmarcha.AI.LanguagesDetection.Azure/README.md)).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.LanguagesDetection.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.LanguagesDetection.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.LanguagesDetection.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.LanguagesDetection.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.LanguagesDetection.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.LanguagesDetection.Abstractions

## How to use

The main way of using this project is by implementing its interfaces in other projects. The main interface is the following:
- [ILanguageDetectionService](./ILanguageDetectionService.cs) is an interface that represents a cognitive service that provides language detections capabilities. Its implementation is available in the NuGet package (see [documentation](../Encamina.Enmarcha.AI.LanguagesDetection.Azure/README.md)).

There are other classes like `LanguageDetectionRequest`, `LanguageDetectionResult` and others, which are basically entities for information exchange.