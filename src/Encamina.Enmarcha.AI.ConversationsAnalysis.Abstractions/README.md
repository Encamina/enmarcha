# AI - Conversation Analysis Abstractions

The primary way to leverage this project is by applying its interfaces within other projects. Currently, there are no implementations in other ENMARCHA projects.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions

## How to use

The primary method of using this project is by implementing its interfaces in other projects (currently, there are no implementations in other ENMARCHA projects). The main interface is the following:
- [IConversationAnalysisService](./IConversationAnalysisService.cs) is an interface that represents a cognitive service that provides conversation analysis capabilities.