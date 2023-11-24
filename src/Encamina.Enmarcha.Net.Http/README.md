# HTTP

This project provides utilities related to HTTP, covering aspects such as the most common media types and common constants, ultimately providing a collection of useful tools for HTTP.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Net.Http](https://www.nuget.org/packages/Encamina.Enmarcha.Net.Http) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Net.Http

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Net.Http](https://www.nuget.org/packages/Encamina.Enmarcha.Net.Http) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Net.Http

## How to use

### Media types

[MediaTypeNames](./MediaTypeNames.cs) provides common media type (formerly known as MIME type) names for file formats and format contents.

```csharp
Console.WriteLine(Encamina.Enmarcha.Net.Http.MediaTypeNames.Image.Jpeg);
// image/jpeg

Console.WriteLine(Encamina.Enmarcha.Net.Http.MediaTypeNames.Application.Json);
// application/json

Console.WriteLine(Encamina.Enmarcha.Net.Http.MediaTypeNames.Text.Csv);
// text/csv

```

[MediaTypeFileExtensionMapper](./MediaTypeFileExtensionMapper.cs) a mapping between file extensions and media types (former MIME). The main difference with [FileExtensionContentTypeProvider](https://github.com/dotnet/aspnetcore/blob/main/src/Middleware/StaticFiles/src/FileExtensionContentTypeProvider.cs) is that this considers various media types per extension, which is usefull for scenarios like for example zip files which can be identified as _application/zip_, _application/zip-compressed_, or _application/x-zip-compressed_.


```csharp
var mediaTypeFileExtensionMapper = new MediaTypeFileExtensionMapper();

// Gets the media types mapped to the given extension
var zipMediaTypes = mediaTypeFileExtensionMapper.GetMediaTypesFromExtension(".zip");
// zipMediaTypes => application/zip, application/zip-compressed, application/x-zip-compressed

// Gets the current mappings between extensions and media types.
zipMediaTypes = mediaTypeFileExtensionMapper.Mappings[".zip"];
// zipMediaTypes => application/zip, application/zip-compressed, application/x-zip-compressed

// Gets the extensions mapped to the given media type.
var zipExtension = mediaTypeFileExtensionMapper.GetExtensionsFromMediaType("application/zip");
// zipExtension => .zip
```

Alternatively, you can create a new instance of `MediaTypeFileExtensionMapper` with your own mappings or merge new mappings with the default ones.

```csharp
var customMappings = new Dictionary<string, IEnumerable<string>>()
{
    { ".odt", new[] { @"application/vnd.oasis.opendocument.text" } },
};

// It only contains the mapping for .odt.
var customMediaTypeFileExtensionMapper = new MediaTypeFileExtensionMapper(customMappings);

// It contains all the default mappings plus the mapping for .odt.
var mergedMediaTypeFileExtensionMapper = new MediaTypeFileExtensionMapper(customMappings, mergeWithDefaultMappings: true);
```

### Constants

A collection of properties that represent common constant values. It includes HTTP headers that can be used as custom HTTP headers, for example, as `Constants.HttpHeaders.CorrelationId` or `Constants.HttpHeaders.SourceUrl`.

### HttpContextExtensions

Extension helper methods when working with an `HttpContext`. 

For example, in a Web API, you may want to retrieve the value of a request header:

```csharp
// ...

HttpContext httpContext;

// ...

// Reads values from the request header. A default value to return if the header is not found.
var myCustomHeaderValue = myhttpContext.ReadValueFromRequestHeader("my-custom-header", defaultValue: "no-custom-header");
```