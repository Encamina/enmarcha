# Entities

Entities project mainly contains implementations of interfaces from the NuGet package Encamina.Enmarcha.Entities.Abstractions.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Entities](https://www.nuget.org/packages/Encamina.Enmarcha.Entities) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Entities

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Entities](https://www.nuget.org/packages/Encamina.Enmarcha.Entities) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Entities

## How to use

### ServiceFactory/ServiceFactoryProvider

- [ServiceFactory&lt;T&gt;](./ServiceFactory{T}.cs) is a factory that can provide valid instances of a specific service of type `T` within a scope. It implements `IServiceFactory<T>`.
- [ServiceFactoryProvider&lt;T&gt;](./ServiceFactoryProvider{T}.cs) is a provider for factories of services of type `T`. It implements `IServiceFactoryProvider<T>`.

An example of use is the [CompletionServiceFactoryProvider](/../Encamina.Enmarcha.AI.OpenAI.Abstractions/Compl) class of the NuGet Encamina.Enmarcha.AI.OpenAI.Abstractions.

### SimpleRetryHelper

[SimpleRetryHelper](./SimpleRetryHelper.cs) is a simple implementation of a helper for retrying failed operations.

Starting from a `Program.cs` or a similar entry point file in your project, add the following code:
```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

services.AddLogging();
services.TryAddTransient<IRetryHelper, SimpleRetryHelper>();
```

And now you can use `IRetryHelper`, for example, with a constructor injection.

```csharp
public class MyClass
{
    private readonly IRetryHelper retryHelper;

    public MyClass(IRetryHelper retryHelper)
    {
        this.retryHelper = retryHelper;
    }

    public async Task DoOperationAsync()
    {
        await retryHelper.RetryOperationAsync(retryTimes: 3, waitTimeMilliseconds: 5000, operation: ApiRestCallAsync);
    }

    private async Task ApiRestCallAsync()
    {
         // ...
    }
}
```
When running DoOperationAsync, the `ApiRestCall` method will be called up to 3 times, with a 5000-millisecond interval between attempts. In case it does not complete successfully, the last exception thrown by `ApiRestCallAsync` will be thrown.