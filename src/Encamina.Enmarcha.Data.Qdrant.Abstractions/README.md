# Semantic Kernel - Abstractions

This project mainly contains abstractions used by other ENMARCHA NuGet packages. Additionally, it also includes some extension methods and utilities primarily related to Qdrant.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Data.Qdrant.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Qdrant.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Data.Qdrant.Abstractions

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Data.Qdrant.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Qdrant.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Data.Qdrant.Abstractions

## How to use

In addition to the abstractions (interfaces, abstract classes, etc.) that have their implementations in other ENMARCHA NuGets, such as [IQdrantSnapshotHandler](./IQdrantSnapshotHandler.cs)/[QdrantSnapshotHandler](../Encamina.Enmarcha.Data.Qdrant/QdrantSnapshotHandler.cs) there are some utilities that can be _used directly_.

### QdrantOptions
```json
// ...
  "QdrantOptions": {
    "Host": "https://sample-qdrant.azurewebsites.net/", // Endpoint protocol and host
    "Port": 6333, // Endpoint port
    "VectorSize": 1536, // Vector size
    "ApiKey": "xxxxxxxxxx" // API Key used by Qdrant as a form of client authentication.
  },
// ...
```
The above section is taken from a JSON file that is typically added to the `appsettings.json`. This provides the necessary configurations ([QdrantOptions](./QdrantOptions.cs)) for `Qdrant`.

Once these configurations are defined, they can be added to the application's configuration.

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddOptions<QdrantOptions>().Bind(builder.Configuration.GetSection(nameof(QdrantOptions)))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

### HttpClientExtensions

This class provides some static methods to configure an `HttpClient` to be used with `Qdrant`.
```csharp
// ...

// You probably want to get this in another way, for example, appsettings.json...
var localQdrantOptions = new QdrantOptions()
{
    Host = new Uri("http://localhost:6333/"),
    Port = 6333,
    VectorSize = 1536
};

httpClient.ConfigureHttpClientForQdrant(localQdrantOptions);
```
The previous code adds what is necessary (headers...) to be able to make requests to the Qdrant REST API.