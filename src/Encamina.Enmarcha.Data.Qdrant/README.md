# Data - Qdrant

The Data Qdrant project contains functionalities related to the [Qdrant](https://qdrant.tech/) vector database.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Data.Qdrant](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Qdrant) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Data.Qdrant

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Data.Qdrant](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Qdrant) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Data.Qdrant

## How to use
### QdrantSnapshotHandler

Qdrant vector database supports creating snapshots, which consist in a `.snapshot` archive file containing the necessary data to restore the collection at the time of the snapshot. This handler allows you to take snapshots of `Qdrant`. First, you need to add the [QdrantOptions](../Encamina.Enmarcha.Data.Qdrant.Abstractions/QdrantOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

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

Starting from a `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
// Entry point
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

services.AddQdrantSnapshotHandler();
```

This extension method will add the implementation of the [IQdrantSnapshotHandler](../Encamina.Enmarcha.Data.Qdrant.Abstractions/IQdrantSnapshotHandler.cs) interface as a singleton, in addition to configuring an `HttpClient` with [QdrantOptions](../Encamina.Enmarcha.Data.Qdrant.Abstractions/QdrantOptions.cs) so that the handler can use the Qdrant REST API. The implementation is [QdrantSnapshotHandler](./QdrantSnapshotHandler.cs). With this, it can resolve the `IQdrantSnapshotHandler` interface and take snapshots:

```csharp
public class MyClass
{
    private readonly IQdrantSnapshotHandler qdrantSnapshotHandler;

    public MyClass(IQdrantSnapshotHandler qdrantSnapshotHandler)
    {
        this.qdrantSnapshotHandler = qdrantSnapshotHandler;
    }

    public async Task TestQdrantSnapshotAsync()
    {
        await qdrantSnapshotHandler.CreateCollectionSnapshotAsync("my-collection-1", CancellationToken.None);
    }
}
```

The previous code creates a snapshot of the 'my-collection-1' collection.