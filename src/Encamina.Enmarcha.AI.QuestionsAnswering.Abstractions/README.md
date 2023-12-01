# AI - Questions Answering Abstractions

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions)](https://www.nuget.org/packages/Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions)

This project mainly contains AI abstractions used by other ENMARCHA NuGet packages. This abstractions include interfaces that have their implementations in other projects, clase bases and entities that represent abstractions for information exchange. The NuGet package `Encamina.Enmarcha.AI.QuestionsAnswering.Azure` contains implementations of this package (see [documentation](../Encamina.Enmarcha.AI.QuestionsAnswering.Azure/README.md)).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions

## How to use

### Metadata

- [IMetadataHandler](./IMetadataHandler.cs) is an interface that represents a handler for metadata and extracts metadata from messages.  It has its implementation with [CachedTableStorageCompositeMetadataHandler](../Encamina.Enmarcha.AI.QuestionsAnswering.Azure/Metadata/CachedTableStorageCompositeMetadataHandler.cs) class.
- [IMetadataProcessor](./IMetadataProcessor.cs) is an interface that represents a processor for metadata handlers.
- [MetadataProcessorBase](./MetadataProcessorBase.cs) is a base class that implements `IMetadataProcessor` and it is responsible for processing `IMetadataHandlers` based on their order.

### Question

- [IQuestionAnsweringService](./IQuestionAnsweringService.cs) is an interface that represents a cognitive service that provides question answering capabilities. It has its implementation with [QuestionAnsweringService](../Encamina.Enmarcha.AI.QuestionsAnswering.Azure/QuestionAnsweringService.cs) class.
- [IQuestionRequestHandler](./IQuestionRequestHandler.cs) is an interface that represents a handler for question requests.
- [IQuestionRequestProcessor](./IQuestionRequestProcessor.cs) is an interface that represents a processor for question requests handlers.
- [QuestionRequestProcessorBase](./QuestionRequestProcessorBase.cs) is a base class that implements `IQuestionRequestProcessor` and it is responsible for processing `IQuestionRequestHandlers` based on their order.
- [IQuestionResultHandler](./IQuestionRequestHandler.cs) is an interface that represents a handler for question results.
- [IQuestionResultProcessor](./IQuestionResultProcessor.cs) is an interface that represents a processor for question results handlers.
- [QuestionResultProcessorBase](./QuestionResultProcessorBase.cs) is a base class that implements `IQuestionResultProcessor` and it is responsible for processing `IQuestionResultHandlers` based on their order.

### Sources

- [ISourcesHandler](./ISourcesHandler.cs) is an interface that represents a handler for sources.
- [ISourcesProcessor](./ISourcesProcessor.cs) is an interface that represents a processor for source handlers.
- [SourcesProcessorBase](./SourcesProcessorBase.cs) is a base class that implements `ISourcesProcessor` and it is responsible for processing `ISourcesHandler` based on their order.

There are other classes like `QuestionRequestOptions`, `MetadataOptions` and others, which are basically entities that store service configurations.