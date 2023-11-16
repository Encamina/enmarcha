# Dependency Injection

This project contains functionalities related to dependency injection. It primarily includes extension methods to simplify and enhance the capabilities of [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/).

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.DependencyInjection](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.DependencyInjection

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.DependencyInjection](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.DependencyInjection

## How to use
### AutoRegisterServiceAttribute

You can use the [AutoRegisterServiceAttribute](./AutoRegisterServiceAttribute.cs) attribute for your types, which enables automatic registration of themselves into the dependency injection container.

```csharp
public interface IFoo
{
    void SayFoo();
}

[AutoRegisterService(ServiceLifetime.Singleton)]
public class ConsoleFoo : IFoo
{
    public void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}
```

and a `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    // ...
});

// ... 

builder.Services.AddAutoRegisterServicesFromAssembly<Program>();
```

The above code adds types decorated with the `AutoRegisterServiceAttribute` (such as `FooBar`) into the service collection from the `Program` assembly.

With this, it can resolve the `IFoo` class with construction injection.

```csharp
public class MyClass
{
    private readonly IFoo foo;

    public MyClass(IFoo foo)
    {
        this.foo = foo;
    }

    public void TestFoo()
    {
        foo.SayFoo();
    }
}
```

or Service Provider

```csharp
var serviceProvider = services.BuildServiceProvider();
var foo = serviceProvider.GetRequiredService<IFoo>();

 foo.SayFoo();
```

#### Configuring ServiceLifetime

You can configure the `ServiceLifetime` to be Singleton, Scoped, or Transient.

```csharp
public interface IFoo
{
    void SayFoo();
}

// Registers ConsoleFoo as Scoped
[AutoRegisterService(ServiceLifetime.Scoped)]
public class ConsoleFoo : IFoo
{
    public void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}
```

#### Registering Alternative Types

You can specify the alternative types with which the class will be registered.

```csharp
public interface IFoo
{
    void SayFoo();
}

public class ConsoleFooBar : IFoo
{
    public virtual void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}

[AutoRegisterService(ServiceLifetime.Scoped, typeof(ConsoleFooBar), typeof(IFoo))]
public class RedConsoleFooBar : ConsoleFooBar
{
    public override void SayFoo()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        base.SayFoo();
        Console.ResetColor();
    }
}
```

In the above code, it specifies that `RedConsoleFooBar` should register the alternative types `ConsoleFooBar` and `IFoo`. This means that when you resolve an instance of type `RedConsoleFooBar`, `ConsoleFooBar`, or `IFoo`, it will be resolved as `RedConsoleFooBar`.

```csharp
// ...

var foo = builder.Services.BuildServiceProvider().GetRequiredService<IFoo>();
var consoleFooBar = builder.Services.BuildServiceProvider().GetRequiredService<ConsoleFooBar>();
var redConsoleFooBar = builder.Services.BuildServiceProvider().GetRequiredService<RedConsoleFooBar>();

bool fooIsRedConsoleFooBar = foo.GetType() == typeof(RedConsoleFooBar); // Is true
bool consoleFooBarIsRedConsoleFooBar = consoleFooBar.GetType() == typeof(RedConsoleFooBar); // Is true
bool fooRedConsoleFooBarIsRedConsoleFooBar = redConsoleFooBar.GetType() == typeof(RedConsoleFooBar); // Is true
```

#### Force only as implementation type

There are some scenarios when a class should be registered only as an implementation type, regardless the interfaces it implements. For this scenarios, sets ForceOnlyAsImplementationType to true.

```csharp
public interface IFoo
{
    void SayFoo();
}

[AutoRegisterService(ServiceLifetime.Scoped, ForceOnlyAsImplementationType = true)]
public class ConsoleFooBar : IFoo
{
    public void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}
```

Now, you can only resolve `ConsoleFooBar` directly using the class itself (`ConsoleFooBar`); attempting to resolve it through its interface will throw an exception.

```csharp
// ...

// Throws System.InvalidOperationException: 'No service for type 'IFoo' has been registered.'
var foo = builder.Services.BuildServiceProvider().GetRequiredService<IFoo>();

// It's OK
var consoleFooBar = builder.Services.BuildServiceProvider().GetRequiredService<ConsoleFooBar>();
```

#### Not registering interfaces

Set `RegisterInterfaces` to false when the interfaces directly implemented by the class should not be registered as service types.

```csharp
public interface IFoo
{
    void SayFoo();
}

[AutoRegisterService(ServiceLifetime.Scoped, RegisterInterfaces = false)]
public class ConsoleFooBar : IFoo
{
    public void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}
```

Now, when attempting to resolve `IFoo` directly, an exception will be thrown.

```csharp
// ...

// Throws System.InvalidOperationException: 'No service for type 'IFoo' has been registered.'
var foo = builder.Services.BuildServiceProvider().GetRequiredService<IFoo>();

// It's OK
var consoleFooBar = builder.Services.BuildServiceProvider().GetRequiredService<ConsoleFooBar>();
```

#### Registering inherited interfaces

Set `IncludeInheritedInterfaces` to true when the interfaces inherited by the class (i.e., implemented by base classes) should be registered as services types.

```csharp
public interface IFoo
{
    void SayFoo();
}

public class ConsoleBaseFooBar : IFoo
{
    public virtual void SayFoo()
    {
    Console.WriteLine("Foo");
    }
}

[AutoRegisterService(ServiceLifetime.Scoped, IncludeInheritedInterfaces = true)]
public class RedConsoleFooBar : ConsoleBaseFooBar
{
    public override void SayFoo()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        base.SayFoo();
        Console.ResetColor();
    }
}
```

Even though `RedConsoleFooBar` does not explicitly specify that it implements `IFoo`, since its base class `ConsoleBaseFooBar` implements it, you can resolve `IFoo` directly.

```csharp
// ...

// It's OK
var foo = builder.Services.BuildServiceProvider().GetRequiredService<IFoo>();
```

### Other functionalities

You can directly register a type or type/implementation by specifying the `ServiceLifetime`.

```csharp
public interface IFoo
{
    void SayFoo();
}

public class ConsoleFooBar : IFoo
{
    public virtual void SayFoo()
    {
        Console.WriteLine("Foo");
    }
}
```
Next, in `Program.cs` or a similar entry point file in your project, add the following code.

```csharp
// Register by type
builder.Services.AddType<ConsoleFooBar>(ServiceLifetime.Singleton);
builder.Services.TryAddType<ConsoleFooBar>(ServiceLifetime.Singleton);

// or TService, TImplementation
builder.Services.AddType<IFoo, ConsoleFooBar>(ServiceLifetime.Singleton);
builder.Services.TryAddType<IFoo, ConsoleFooBar>(ServiceLifetime.Singleton);

// or with implementation Instance
builder.Services.TryAddType<IFoo>(ServiceLifetime.Singleton, new ConsoleFooBar());

// or with factory method
builder.Services.TryAddType<IFoo, ConsoleFooBar>(ServiceLifetime.Singleton, provider => new ConsoleFooBar(provider.GetRequiredService<IDependency>()));

// ... the TryAddType and AddType methods can be used in various flavors to register services into an IServiceCollection.
```