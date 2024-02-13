# OpenApi - Swashbuckle

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle)](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle)

This package provides extensions and filters for OpenAPI (Swagger) with version support using [Swashbuckle](https://learn.microsoft.com/es-es/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-8.0).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle

## How to use

## IServiceCollectionExtensions

`IServiceCollectionExtensions` is a static class that provides extension methods for `IServiceCollection` to add and configure services for OpenAPI (Swagger) with version support.

### Usage

You can use the methods in this class as follows:

```csharp
IServiceCollection services = new ServiceCollection();
IConfiguration configuration = new ConfigurationBuilder().Build();

services.AddVersionSwaggerGenConfigureOptions(configuration);
```

```csharp
IServiceCollection services = new ServiceCollection();
Action<VersionSwaggerGenOptions> options = opts =>
{
    opts.Title = "CUSTOM REST API"; // Title for the OpenAPI document when using version support.
};
services.AddVersionSwaggerGenConfigureOptions(options);
```

### Methods

- `AddVersionSwaggerGenConfigureOptions(this IServiceCollection services, IConfiguration configuration)`: Adds version support for the generation of OpenAPI documents using configuration parameters from the current set of key-value application configuration. It returns the `IServiceCollection` for chaining additional calls.

- `AddVersionSwaggerGenConfigureOptions(this IServiceCollection services, Action<VersionSwaggerGenOptions> options)`: Adds version support for the generation of OpenAPI documents using an action to configure option parameters. It returns the `IServiceCollection` for chaining additional calls.

- `AddVersionSwaggerGenConfigureOptions(this IServiceCollection services)`: Adds version support for the generation of OpenAPI documents. It returns the `IServiceCollection` for chaining additional calls.

## Filters

### ApiKeyHeaderOperationFilter

`ApiKeyHeaderOperationFilter` is a class that implements the `IOperationFilter` interface. It's used to add an API key header to the OpenAPI (Swagger) documentation.

#### Usage

To use this class, you need to create an instance and pass it to the Swagger configuration:

```csharp
services.AddSwaggerGen(c =>
{
    c.OperationFilter<ApiKeyHeaderOperationFilter>("YourHeaderApiKey", "YourDescription");
});
```

#### Constructor

- `ApiKeyHeaderOperationFilter(string headerApiKey)`: Initializes a new instance of the class with the given header API key name and a default description.

- `ApiKeyHeaderOperationFilter(string headerApiKey, string description)`: Initializes a new instance of the class with the given header API key name and description.

#### Methods

- `Apply(OpenApiOperation operation, OperationFilterContext context)`: Applies the operation filter to the OpenAPI operation. It adds a new `OpenApiParameter` to the operation's parameters, representing the API key header.

### DefaultValuesOperationFilter

This C# code defines a class DefaultValuesOperationFilter that implements the IOperationFilter interface. This class is used to enrich the generated OpenAPI (Swagger) documents with default values for parameters.

`DefaultValuesOperationFilter` is a class that implements the `IOperationFilter` interface. It's used to enrich the generated OpenAPI (Swagger) documents with default values for parameters.

#### Usage

To use this class, you need to create an instance and pass it to the Swagger configuration:

```csharp
services.AddSwaggerGen(c =>
{
    c.OperationFilter<DefaultValuesOperationFilter>();
});
```

#### Methods

- `Apply(OpenApiOperation operation, OperationFilterContext context)`: Applies the operation filter to the OpenAPI operation. It adds default values to the operation's parameters and removes unsupported content types from the operation's responses.

## VersionSwaggerGenConfigureOptions

This C# code defines a class VersionSwaggerGenConfigureOptions that implements the IConfigureOptions<SwaggerGenOptions> interface. This class is used to configure Swagger generation options for versioned OpenAPI documents.

`VersionSwaggerGenConfigureOptions` is a class that implements the `IConfigureOptions<SwaggerGenOptions>` interface. It's used to configure Swagger generation options for versioned OpenAPI documents.

### Usage

To use this class, you need to create an instance and pass it to the Swagger configuration:

```csharp
services.AddSwaggerGen(c =>
{
    c.Configure<VersionSwaggerGenConfigureOptions>();
});
```

### Constructor

- `VersionSwaggerGenConfigureOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider, IOptions<VersionSwaggerGenOptions> options)`: Initializes a new instance of the class with the given API version description provider and configuration options.

### Methods

- `Configure(SwaggerGenOptions options)`: Configures the Swagger generation options. It adds a new `OpenApiInfo` to the Swagger documentation for each API version.
