# AI - Abstractions

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AI.Abstractions)](https://www.nuget.org/packages/Encamina.Enmarcha.AI.Abstractions)

This project houses AI abstractions that are utilized by other ENMARCHA NuGet packages. These abstractions primarily include interfaces, which have their implementations in separate projects, and entities that represent the configurations of various services. The Encamina.Enmarcha.AI NuGet package encompasses the implementations of these abstractions. For more details, please refer to the [documentation](../Encamina.Enmarcha.AI/README.md).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.Abstractions

## How to use

The only way to use this project is through its interfaces, using some of their implementations generated in other projects. The main interfaces are the following:
- [ICognitiveServiceFactory](./ICognitiveServiceFactory.cs) is an interface that represents a factory that can provide valid instances of a specific cognitive service type.
- [IDocumentContentExtractor](./IDocumentContentExtractor.cs) is an interface that represents a document content extractor, which extracts its content from a stream. It has its implementation in Encamina.Enmarcha.SemanticKernel.Connectors.Document NuGet (see [documentation](../Encamina.Enmarcha.SemanticKernel.Connectors.Document/README.md)).
- [ITextSplitter](./ITextSplitter.cs) is an interface that represents a text splitter, which splits a text into chunks of text. It has its implementation in Encamina.Enmarcha.AI NuGet (see [documentation](../Encamina.Enmarcha.AI/README.md)).

There are other classes like `TextSplitterOptions`, `CognitiveServiceOptionsBase` and more, which are entities that store service configurations.