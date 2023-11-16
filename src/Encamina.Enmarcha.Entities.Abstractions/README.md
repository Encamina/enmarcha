# Entities - Abstractions

Entities Abstractions is a project that primarily contains abstractions used by other NuGet packages in ENMARCHA. These abstractions represent properties or characteristics that entities can have.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Entities.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Entities.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Entities.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Entities.Abstractions

## How to use

### HandlerManagerBase

[HandlerManagerBase](./HandlerManagerBase.cs) is a base class for a handlers manager. It serves as a foundation for handling various types of handlers. You can use it as a starting point for creating specialized handler manager classes.

```csharp
public class CustomHandler
{
}

public class MyCustomHandlerManager : HandlerManagerBase<CustomHandler>
{
    public MyCustomHandlerManager(IEnumerable<CustomHandler> handlers) : base(handlers)
    {
    }

    public void LoopHandlers()
    {
        foreach (var handler in Handlers)
        {
            // ...
        }
    }
}
```
In the previous code, a class is created based on `HandlerManagerBase` where all the `Handlers` are looped in the `LoopHandlers` method.

### HandlerProcessTimes

[HandlerProcessTimes](./HandlerProcessTimes.cs) is an Enum that represents the process time when a specific handler should be executed. It has 4 possible values:
- None: Indicates the handled should never be processed.
- Begin: Indicates the handler should be processed at the beginning of the turn context.
- End: Indicates the handler should be processed at the ending of the turn context.
- Both: Indicates the handler should be processed at both, the beginning and the ending of the turn context.

### IIdentifiable/IdentifiableBase

- [IIdentifiable](./IIdentifiable.cs) is an interface that represents a uniquely identifiable entity.
- [IIdentifiable&lt;T&gt;](./IIdentifiable{T}.cs) is an interface that represents a uniquely identifiable entity.
- [IdentifiableBase](./IdentifiableBase.cs) is a base class for entities that must have a property that represents a unique identifier. It implements `IIdentifiable`.
- [IdentifiableBase&lt;T&gt;](./IIdentifiable{T}.cs) is a base class for entities that must have a property that represents a unique identifier. It implements `IIdentifiable<T>`.

```csharp
public class MyCustomIdentifiableEntity : IIdentifiable
{
    public object Id { get; }
}

public class MyCustomTypedIdentifiableEntity : IIdentifiable<string>
{
    object IIdentifiable.Id => Id;

    public string Id { get; }
}

public class MyCustomIdentifiableBaseEntity : IdentifiableBase
{

}

public class MyCustomTypedIdentifiableBaseEntity : IdentifiableBase<string>
{

}
```

### IIdentifiableValuable

[IIdentifiableValuable](./IIdentifiableValuable.cs) is an interface that represents entities with an unique identifier and value. It implements `IIdentifiable<T>` and `IValuable<T>`.
```csharp
public class MyCustomIdentifiableAndValuableEntity : IIdentifiableValuable<Guid, string>
{    
    public string Value { get; }

    object IIdentifiable.Id => Id;

    public Guid Id { get; }
}
```

### IIntendable

[IIntendable](./IIntendable.cs) is an interface that represents an intendable entity. In other words, and entity that has an intent.
```csharp
public class MyCustomIntendableEntity : IIntendable
{
    public string Intent { get; }
}
```

### INameable

[INameable](./INameable.cs) is an interface that represents a nameable entity.
```csharp
public class MyCustomNameableEntity : INameable
{
    public string Name { get; }
}
```

### INameableIdentifiable

[INameableIdentifiable](./INameableIdentifiable.cs) is an interface that represents entities with a name and an unique identifier. It implements `INameable` and `IIdentifiable<T>`.
```csharp
public class MyCustomNameableAndIdentifiableEntity : INameableIdentifiable<Guid>
{
    public string Name { get; }

    object IIdentifiable.Id => Id;

    public Guid Id { get; }
}
```

### INameableIdentifiableValuable

[INameableIdentifiableValuable](./INameableIdentifiableValuable.cs) is an interface that represents entities with a name, an unique identifier and value. It implements `INameable`, `IIdentifiable<T>` and `IValuable<T>`.
```csharp
public class MyCustomNameableAndIdentifiableAndValuableEntity : INameableIdentifiableValuable<Guid, int>
{
    public string Name { get; }

    object IIdentifiable.Id => Id;

    public Guid Id { get; }

    public int Value { get; }
}
```

### INameableValuable

[INameableValuable](./INameableValuable.cs) is an interface that represents entities with a name and value. It implements `INameable` and `IValuable<T>`. This is an alternative to KeyValuePair and KeyValuePair{TKey, TValue}. The latter two do not support inheritance because KeyValuePair is a static class and KeyValuePair{TKey, TValue} is a structure.

```csharp
public class MyCustomNameableAndValuableEntity : INameableValuable<int>
{
    public string Name { get; }

    public int Value { get; }
}
```

### IOrderable

[IOrderable](./IOrderable.cs) is an interface that represents an orderable type.
```csharp
public class MyCustomOrderableEntity : IOrderable
{    
    public int Order { get; }
}
```

### IRetryHelper

[IRetryHelper](./IRetryHelper.cs) is an interface that represents a helper for retrying failed operations.
```csharp
public class MyCustomRetryHelper : IRetryHelper
{
    public Task RetryOperationAsync(int retryTimes, int waitTimeMilliseconds, Func<Task> operation)
    {
        // ...
        // Implementation of retry operation
        // ...

        return Task.CompletedTask;
    }
}
```

There is an implementation of this interface in the NuGet package Encamina.Enmarcha.Entities, [SimpleRetryHelper](../Encamina.Enmarcha.Entities/SimpleRetryHelper.cs).


### IServiceFactory/IServiceFactoryProvider

- [IServiceFactory&lt;T&gt;](./IServiceFactory{T}.cs) is an interface that represents a factory that can provide valid instances of a specific service of type `T` within a scope. There is an implementation of this interface in the NuGet package Encamina.Enmarcha.Entities, [ServiceFactory&lt;T&gt;](../Encamina.Enmarcha.Entities/ServiceFactory{T}.cs).
- [IServiceFactoryProvider&lt;T&gt;](./IServiceFactoryProvider{T}.cs) is an interface that represents a provider for factories of services of type `T`. There is an implementation of this interface in the NuGet package Encamina.Enmarcha.Entities, [ServiceFactoryProvider&lt;T&gt;](../Encamina.Enmarcha.Entities/ServiceFactoryProvider{T}.cs).

### IValidableEntity/ValidableEntity

- [IValidableEntity](./IValidableEntity.cs) is an interface that represents entities that provides their own validation mechanism.
- [ValidableEntity](./ValidableEntity.cs) is a base class that represents an entity that can be validated by itself. It implements `IValidableEntity`. 
```csharp
public class MyCustomValidableEntity : ValidableEntity
{
    public string SomeProperty { get; set; }
    
    public override IEnumerable<string> Validate()
    {
        var validationResults = base.Validate().ToList();

        if (string.IsNullOrWhiteSpace(SomeProperty))
        {
            validationResults.Add("SomeProperty must not be empty.");
        }

        return validationResults;
    }
}
```

### IValuable

[IValuable](./IValuable.cs) is an interface that represents an entity with value.
```csharp
public class MyCustomValuableEntity : IValuable<double>
{    
    public double Value { get; }
}
```

### NameableHandlerManagerBase

[NameableHandlerManagerBase](./NameableHandlerManagerBase.cs) is a base class for a handlers manager that uses handlers that implements the `INameable` interface.
```csharp
public class CustomNameableHandler : INameable
{
    public CustomNameableHandler(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class MyCustomHandlerManager : NameableHandlerManagerBase<CustomNameableHandler>
{
    public MyCustomHandlerManager(IEnumerable<CustomNameableHandler> handlers) : base(handlers)
    {
    }

    public void LoopHandlers()
    {
        foreach (var handler in Handlers)
        {
            Console.WriteLine($"{handler.Value}");
        }
    }
}
```
In the code above, a class is created based on `NameableHandlerManagerBase` where all the `Handlers` are looped in the `LoopHandlers` method.

### OrderableHandlerManagerBase

[OrderableHandlerManagerBase](./OrderableHandlerManagerBase.cs) is a base class for a handlers manager that uses handlers that implements the `IOrderable` interface.
```csharp
public class CustomOrderableHandler : IOrderable
{
    public CustomOrderableHandler(int order)
    {
        Order = order;
    }

    public int Order{ get; }
}

public class MyCustomHandlerManager : OrderableHandlerManagerBase<CustomOrderableHandler>
{
    public MyCustomHandlerManager(IEnumerable<CustomOrderableHandler> handlers) : base(handlers)
    {
    }

    public void LoopHandlers()
    {
        foreach (var handler in Handlers)
        {
            Console.WriteLine($"{handler.Order}");
        }
    }
}
```

In the code above, a class is created based on `OrderableHandlerManagerBase` where all the `Handlers` are looped in order by the `LoopHandlers` method.