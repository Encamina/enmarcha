# AI - Intents Prediction Abstractions

This project houses Intents Prediction abstractions uilized by other ENMARCHA NuGet packages. These abstractions primarily include interfaces that have their implementations in separate projects, and entities that represent abstractions for information exchange. The NuGet package `Encamina.Enmarcha.AI.IntentsPrediction.Azure` encompasses implementations of this package (see [documentation](../Encamina.Enmarcha.AI.IntentsPrediction.Azure/README.md)).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.IntentsPrediction.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.IntentsPrediction.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.IntentsPrediction.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.IntentsPrediction.Abstractions

## How to use

The main way of utilizing this project is by implementing its interfaces in other projects. The main interface is the following:
- [IIntentPredictionService](./IIntentPredictionService.cs) is an interface that represents a cognitive service that provides intent prediction capabilities. Its implementation is available in the NuGet package (see [documentation](../Encamina.Enmarcha.AI.IntentsPrediction.Azure/README.md)).

There are other classes like `IntentPredictionRequest`, `IntentPredictionResult` and others, which are basically entities for information exchange.