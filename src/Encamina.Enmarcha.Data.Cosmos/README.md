# Data - Cosmos

Cosmos Data project primarily contains Cosmos DB implementations based on the abstractions provided by [Encamina.Enmarcha.Data.Abstractions](../Encamina.Enmarcha.Data.Abstractions/README.md), as well as some other utilities related to Cosmos DB.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Data.Cosmos](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Cosmos) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Data.Cosmos

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Data.Cosmos](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Cosmos) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Data.Cosmos

## How to use

In the following example, we will demonstrate how to configure and add an Cosmos DB implementation of the [IAsyncRepository](../Encamina.Enmarcha.Data.Abstractions/IAsyncRepository.cs) interface to the `ServiceCollection`, based on the [CosmosRepository](./CosmosRepository{T}.cs).

```csharp
public class Foo 
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}
```

First, you need to add the [CosmosOptions](./CosmosOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "CosmosOptions": {
        "AuthKey": "<Your-AuthKey>", // Authentication key required to connect with Azure Cosmos DB
        "Database": "<Your-Database>", // Database name to connect with Azure Cosmos DB
        "Endpoint": "<Your-Endpoint>", // Azure Cosmos DB service endpoint to use
        // ...
    },
    "CosmosDBContainerName": "<Your-Cosmos-DB-Container-Name>",
    // ...
  }
```

Next, in `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile(@"appsettings.json", optional: true, reloadOnChange: true);  

builder.Services.AddCosmos(builder.Configuration);
builder.Services.AddScoped<IAsyncRepository<Foo>>(sp => sp.GetRequiredService<ICosmosRepositoryFactory>()
    .Create<Foo>(builder.Configuration.GetValue<string>("CosmosDBContainerName")));
```

And now, you can resolve the `IAsyncRepository<Foo>` interface with construction injection:

```csharp
public class MyClass
{
    private readonly IAsyncRepository<Foo> fooRepository;

    public MyClass(IAsyncRepository<Foo> fooRepository)
    {
        this.fooRepository = fooRepository;
    }

    public async Task TestAddFooAsync(CancellationToken cts)
    {
        await fooRepository.AddAsync(new Foo() { Text = "Foo1", Id = Guid.NewGuid().ToString()}, cts);        
    }
}
```

Within the NuGet package, you will find some extension methods related to Cosmos DB in [FeedIteratorExtensions](./Extensions/FeedIteratorExtensions.cs).