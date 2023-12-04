# ASP.NET - MVC Formatters

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.AspNet.Mvc.Formatters)](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.Mvc.Formatters)

ASP.NET MVC Formatters is a project that provides input and output formatters for enhancing the functionality of ASP.NET MVC applications. These formatters are designed to simplify the handling of data input and output, making it more convenient and efficient for developers to work with MVC projects.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AspNet.Mvc.Formatters](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.Mvc.Formatters) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AspNet.Mvc.Formatters

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AspNet.Mvc.Formatters](https://www.nuget.org/packages/Encamina.Enmarcha.AspNet.Mvc.Formatters) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AspNet.Mvc.Formatters

## How to use

### Output formatters

Starting from a `Program.cs` or a similar entry point file in your MVC project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services
    .AddControllers()
    // Adds csv output formatter
    .AddCsvOutputFormatter(options =>
    {
        options.Delimiter = ','; // The values delimiter or separator
        options.UseHeader = true; // A flag indicating whether the comma separated values should include a header (line) or not
        options.Encoding = System.Text.Encoding.UTF8.WebName; // Encoding
    })
    // Adds csv output formatter
    .AddPdfOutputFormatter();

// ...

```

The extension methods `AddCsvOutputFormatter` and `AddPdfOutputFormatter` handle the configuration to enable any endpoint to support returning data not only in the default JSON format but also in CSV or PDF formats.

```
curl --location 'https://my-endpoint.net/controller' \
--header 'Accept: text/csv'
# Response: 200 OK with CSV data

curl --location 'https://my-endpoint.net/controller' \
--header 'Accept: application/pdf'
# Response: 200 OK with PDF data
```

### Input formatters

Starting from a `Program.cs` or a similar entry point file in your MVC project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services
    .AddControllers()
    // Adds csv output formatter
    .AddCsvInputFormatter(options =>
    {
        options.Delimiter = ','; // The values delimiter or separator
        options.UseHeader = true; // A flag indicating whether the comma separated values should include a header (line) or not
        options.Encoding = System.Text.Encoding.UTF8.WebName; // Encoding
    });

// ...

```

The extension method `AddCsvInputFormatter` is responsible for configuring everything necessary to enable any endpoint to support receiving information through the body in CSV format.

```
curl --location 'https://localhost:7184/weatherforecast' \
--header 'Content-Type: text/csv' \
--data 'Foo,Bar
Foo Value,100
Other foo Value, 150'
# Response: 200 OK
```