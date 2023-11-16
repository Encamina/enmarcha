# ASP.NET - MVC

ASP.NET MVC is a project designed to simplify the configuration and usage of MVC projects. It contains utilities related to authentication, authorization, binders, and more, with a focus on streamlining the use of common functionalities and reducing the necessary code for their implementation.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.AspNet.Mvc](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.AspNet.Mvc

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.AspNet.Mvc](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.AspNet.Mvc

## How to use
### Basic Authorization

First, you need to add the [BasicApiKeyOptions](./BasicApiKeyOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "BasicApiKeyOptions": {
        "ApiKeys": {     
        // Dictionary that relates an API key client unique identifier its expected API key. 
        "WeatherAuthoringKey": "123456",
        "AnimalsAuthoringKey": "00000",
        }
    },
    // ...
  }
```

Next, in `Program.cs` or a similar entry point file in your MVC project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

// ...

builder.Services
    .AddControllers()
    .AddApiKeyAuthorizationFilter();

builder.Services
    .AddBasicApiKeyAuthorization(builder.Configuration);

// ...
```

In the JSON, different API keys and their values are defined. Now, it is only necessary to decorate any controller with the `ApiKey` attribute, and automatically, each time a request is made, it must contain the `x-api-key` header with the corresponding value of the key referenced by the attribute.

```csharp
[ApiController]
[ApiKey("WeatherAuthoringKey")]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public WeatherForecastController()
    {
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```

As observed, the `WeatherForecastController` controller is decorated with the `ApiKey` attribute, which refers to `WeatherAuthoringKey`. Therefore, any request that does not contain the 'x-api-key' header with the value `123456` will result in an Unauthorized 401 error.

```
curl --location 'https://yourendpoint.net/weatherforecast' \
--header 'x-api-key: 123456'
# Response: 200 OK

curl --location 'https://yourendpoint.net/weatherforecast' \
--header 'x-api-key: 465465456'
# Response: 401 Unauthorized

curl --location 'https://yourendpoint.net/weatherforecast' \
# Response: 401 Unauthorized
```

### Authentication
There are several extension methods that facilitate the configuration of Authentication, both OpenID Connect and JWT-bearer. 

#### JWT-bearer with Azure Active Directory authentication

First, you need to add the [AzureActiveDirectoryOptions](./Authentication/AzureActiveDirectoryOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "AzureActiveDirectoryOptions": {
        "ClientId" : "", // Azure Active Directory client's ID (sometimes also called Application ID).
        "ClientSecret" : "", // Client's secret on Azure Active Directory.
        "Instance" : "", // Instance
        "Domain" : "", // Domain
        "TenantId" : "", // Azure's tenant ID
        "CallbackPath" : "" // Callback path, which sometimes is just an URL
    },
    // ...
  }
```

Next, in a `Program.cs` or a similar entry point file in your MVC project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// ...

builder.Services
    .AddAuthentication(options =>
    {
        // ...
    })
    .AddJwtBearerAuthentication();

// ...
```

#### OpenId Connect with Azure Active Directory authentication

First, you need to add the [AzureActiveDirectoryOptions](./Authentication/AzureActiveDirectoryOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings would appear using the appsettings.json file:

```json
  {
    // ...
    "AzureActiveDirectoryOptions": {
        "ClientId" : "", // Azure Active Directory client's ID (sometimes also called Application ID).
        "ClientSecret" : "", // Client's secret on Azure Active Directory.
        "Instance" : "", // Instance
        "Domain" : "", // Domain
        "TenantId" : "", // Azure's tenant ID
        "CallbackPath" : "" // Callback path, which sometimes is just an URL
    },
    // ...
  }
```

Next, in a `Program.cs` or a similar entry point file in your MVC project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// ...

builder.Services
    .AddAuthentication(options =>
    {
        // ...
    })
    .AddOpenIdConnectAuthentication();

// ...
```

### Binding
#### CustomDateTimeModelBinderProvider

[CustomDateTimeModelBinderProvider](./Bindings/CustomDateTimeModelBinderProvider.cs) provides custom `DateTime` model binders as a valid `IModelBinderProvider` that can be registered in MvcOptions.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services
    .AddMvcCore(options =>
    {
        // ...

        options.ModelBinderProviders.Insert(0, new CustomDateTimeModelBinderProvider());

        // ...
    });

// ...
```

Now, if we have, for example, an endpoint that receives a `DateTime`, it can be parsed from different `string` formats.

```csharp
// ... Controller method

[HttpGet]
public string GetDatetime(DateTime? time)
{
    return time.ToString();
}
```

```
curl --location 'https://yourendpoint/test?time=20051203'
# Response: 200 OK, 12/3/2005 12:00:00 AM
```
#### FormToDictionaryModelBinder

[FormToDictionaryModelBinder](./Bindings/FormToDictionaryModelBinder.cs) binds the incoming form in the request to a dictionary. It checks if the request has a form content type, and if so, it retrieves the form data. If the form data contains the specified field name, it attempts to deserialize the corresponding value into a dictionary of type `<TKey, TValue>` using JSON deserialization.