# Services - Abstractions

Services Abstractions is a project that provides useful abstractions for your services. It contains classes and interfaces so that you can create an execution context and gather information that you can later consume from your services.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Services.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Services.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Services.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Services.Abstractions

## How to use

### ExecutionContext/ExecutionContextTemplate

This is a set of base classes and interfaces that are useful for creating an execution context that aggregates information and can be consumed from any point in your application. This way, the information from the execution context is centralized in a single point.

- [IExecutionContext](./IExecutionContext.cs) is an interface that represents a basic execution.
- [ExecutionContext](./ExecutionContext.cs) is a base class for basic execution contexts.
- [IExecutionContext&lt;T&gt;](./IExecutionContext{T}.cs) is an interface that represents a basic execution context for a specific type.
- [ExecutionContext&lt;T&gt;](./ExecutionContext{T}.cs) is a base class for basic execution contexts for a specific type.
- [IExecutionContextTemplate](./IExecutionContextTemplate.cs) is an interface that represents the template that can be used to create or configure an `IExecutionContext`.
- [ExecutionContextTemplate](./ExecutionContextTemplate.cs) is a base class for a template that can be used to create or configure an `IExecutionContext`.

To illustrate the usage, let's create an execution context that can be used in a Web API. In addition to the basic information, a new field called `FooInformation` will be added. Some of the constants referenced in the following code block belong to the NuGet package [Encamina.Enmarcha.Net.Http](../Encamina.Enmarcha.Net.Http/README.md).

```csharp
public interface IApiExecutionContextTemplate : IExecutionContextTemplate
{
    string FooInformation { get; init; }
}

public class ApiExecutionContextTemplate: ExecutionContextTemplate, IApiExecutionContextTemplate
{
    public ApiExecutionContextTemplate(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext;
        CorrelationCallId = httpContext.ReadValueFromRequestHeader(Constants.HttpHeaders.CorrelationCallId, Guid.NewGuid().ToString());
        CorrelationId = httpContext.ReadValueFromRequestHeader(Constants.HttpHeaders.CorrelationId, httpContext.TraceIdentifier);
        CancellationToken = httpContext.RequestAborted;
        CultureInfo = CultureInfo.CurrentCulture;
        FooInformation = "Bar";
    }

    public string FooInformation { get; init; }
}

public interface IApiExecutionContext<T> : IApiExecutionContextTemplate, IExecutionContext<T>
    where T : class
{

}

public class ApiExecutionContext<T> : ExecutionContext<T>, IApiExecutionContext<T> where T : class
{
    public ApiExecutionContext(IApiExecutionContextTemplate template, ILogger<T> logger) : base(template, logger)
    {
        FooInformation = template.FooInformation;
    }

    public string FooInformation { get; init; }
}
```

In the code above, `ApiExecutionContextTemplate` is created to contain the required information. Subsequently, this template is received in ApiExecutionContext, which will be the class passed to the controllers.

The next step is, at some point in the Program.cs or a similar entry point file in your project, to add the following code to configure the IoC Container.

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ...

builder.Services.AddLogging();

// ...

builder.Services.AddExecutionContext(
    typeof(IApiExecutionContext<>),
    typeof(ApiExecutionContext<>),
    typeof(IApiExecutionContextTemplate),
    typeof(ApiExecutionContextTemplate));

// ...
```

The `AddExecutionContext` method is responsible for validating and configuring everything necessary as `Scoped`. Now, everything is set up to receive the execution context in a controller.

```csharp
[ApiController]
[Route(@"api/[controller]")]
public class MyCustomController : Controller
{
    private readonly IApiExecutionContext<MyCustomController> executionContext;
    
    public MyCustomController(IApiExecutionContext<MyCustomController> executionContext)
    {
        this.executionContext = executionContext;
    }
   
    [HttpGet("say_hello")]
    [ActionName(nameof(SayHello))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SayHello(CancellationToken cancellationToken)
    {
        // ...

        // Now, we can access executionContext, and we have all the information related to this execution context.
        executionContext.Logger.LogInformation($"Logger from {nameof(SayHello)} method with {executionContext.CorrelationId} and {executionContext.CultureInfo.TwoLetterISOLanguageName}");
        executionContext.Logger.LogInformation($"FooInformation is {executionContext.FooInformation}");
        
        // ...

        return Ok("Hello");
    }
}
```