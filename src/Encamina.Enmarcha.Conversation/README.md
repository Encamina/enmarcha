# Conversation

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Conversation)](https://www.nuget.org/packages/Encamina.Enmarcha.Conversation)

Conversation is a project that primarily contains cross-cutting utilities that can be used in conversational applications.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Conversation](https://www.nuget.org/packages/Encamina.Enmarcha.Conversation) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Conversation

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Conversation](https://www.nuget.org/packages/Encamina.Enmarcha.Conversation) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Conversation

## How to use
Below are some of the most important utilities.

### TableStorageResponseProvider

`TableStorageResponseProvider` is a class that provides responses based on values configured and stored in an Azure Table Storage. It implements the `IIntentResponsesProvider` interface.

#### Usage

To use this class, you need to create an instance and provide the necessary parameters:

```csharp
string tableConnectionString = "Your Azure Table Storage connection string";
string tableName = "Your table name";
string defaultLocale = "en-US";
string intentCounterSeparator = "-";
double cacheAbsoluteExpirationSeconds = 86400;
IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

var responseProvider = new TableStorageResponseProvider(tableConnectionString, tableName, defaultLocale, intentCounterSeparator, cacheAbsoluteExpirationSeconds, memoryCache);
```
You can then use this instance to get responses:

```csharp
var responses = await responseProvider.GetResponsesAsync("YourIntent", "en-US");
```

#### Constructor

- `TableStorageResponseProvider`: Initializes a new instance of the class. It takes a table connection string, a table name, a default locale, an intent counter separator, a cache absolute expiration time in seconds, and an `IMemoryCache`.
 
#### IIntentResponsesProvider Methods

As an implementation of the `IIntentResponsesProvider` interface, this class should implement methods such as `GetResponsesAsync`

