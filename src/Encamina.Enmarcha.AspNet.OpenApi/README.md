# OpenApi

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AspNet.OpenApi)](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi)

This package provides extensions and types to work with OpenAPI (a.k.a. Swagger).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AspNet.OpenApi](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AspNet.OpenApi

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AspNet.OpenApi](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.OpenApi) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AspNet.OpenApi

## How to use

### Middlewares

#### GroupNameKeyAuthorizationMiddleware

This middleware is used to block access to an OpenAPI document or specification b y using a pre-shared key that must be provided in the request header per specification.

To configure, use the `GroupNameKeyAuthenticationOptions` class.

When requesting an OpenAPI specification, identified by its group name, the client must provide the pre-shared key in the request header. Otherwise, the request will fail with an `HTTP 401 UNAUTHORIZED` response. 

### Extensions

#### IApplicationBuilderExtensions

The `IApplicationBuilderExtensions` is a static class that provides extension methods for `IApplicationBuilder`, usually to add and configure middlewares.

Using it is quite simple, just get an instance of an `IApplicationBuilder` and call any of the available extension methods.

```csharp

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    ContentRootPath = Directory.GetCurrentDirectory(),
});

// ...

var app = builder.Build();

app.UseSwaggerGroupNameKeyAuthorization(options =>
{
    // Enable only if not in Development environment. Allow developers to test without keys...
    options.IsEnabled = !builder.Environment.IsDevelopment(); 
});

// ...
```

Currently, available extension methods are:

- `UseSwaggerGroupNameKeyAuthorization(this IApplicationBuilder app, SwaggerGroupNameKeyAuthenticationOptions options)`: adds the `GroupNameKeyAuthorizationMiddleware` middleware using provided options.
- `UseSwaggerGroupNameKeyAuthorization(this IApplicationBuilder app, Action<SwaggerGroupNameKeyAuthenticationOptions>? setupAction = null)`: adds the `GroupNameKeyAuthorizationMiddleware` middleware configuring options inline.

**Important**: Make sure that `UseSwaggerGroupNameKeyAuthorization` is added before any `UseSwagger()`, `UseSwaggerUI()`, or any other OpenAPI implementation middleware call, so that authorization middleware is called before accessing the OpenAPI document.
