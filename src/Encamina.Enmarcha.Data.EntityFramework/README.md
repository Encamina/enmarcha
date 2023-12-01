# Data - Entity Framework

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Data.EntityFramework)](https://www.nuget.org/packages/Encamina.Enmarcha.Data.EntityFramework)

Entity Framework Data project primarily contains Entity Framework implementations based on the abstractions provided by [Encamina.Enmarcha.Data.Abstractions](../Encamina.Enmarcha.Data.Abstractions/README.md), as well as some other utilities related to Entity Framework.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Data.EntityFramework](https://www.nuget.org/packages/Encamina.Enmarcha.Data.EntityFramework) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Data.EntityFramework

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Data.EntityFramework](https://www.nuget.org/packages/Encamina.Enmarcha.Data.EntityFramework) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Data.EntityFramework

## How to use

In the following example, we will demonstrate how to configure and add an Entity Framework implementation of the [IFullUnitOfWork](../Encamina.Enmarcha.Data.Abstractions/IFullUnitOfWork.cs) interface to the `ServiceCollection`, based on the [FullUnitOfWork](./FullUnitOfWork.cs), using the following `DbContext`.

```csharp
public class Foo
{
    public int Id { get; set; }
    public string Text { get; set; }
}

public class Bar
{
    public int Id { get; set; }
    public string Text { get; set; }
}

public class MyDBContext : DbContext
{
    public DbSet<Foo> Foos { get; set; }
    public DbSet<Bar> Bars { get; set; }

}
```

The next step is to inherit from `FullUnitOfWork` and create a custom class that receives an instance of `MyDBContext` in its constructor.

```csharp
public class MyFullUnitOfWork: FullUnitOfWork, IFullUnitOfWork
{
    public MyFullUnitOfWork(MyDBContext dbContext) 
        : base(dbContext)
    {
    }
}
```

In the `Program.cs` or a similar entry point file in your project, add the following code:

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

services.AddDbContext<DatabaseContext>(opt =>
{
    opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")); // Configures your provider
}, ServiceLifetime.Scoped);

// ...

builder.Services.AddScoped<IFullUnitOfWork, MyFullUnitOfWork>();
```

And now, we can resolve the `IFullUnitOfWork` interface with construction injection:

```csharp
public class MyClass
{
    private readonly IFullUnitOfWork uow;

    public MyClass(IFullUnitOfWork uow)
    {
        this.uow = uow;
    }

    public async Task TestAddFooAndBarAndSaveAsync(CancellationToken cts)
    {
        var fooRepository = uow.GetAsyncRepository<Foo>();
        var barRepository = uow.GetAsyncRepository<Bar>();

        await fooRepository.AddAsync(new Foo() { Text = "Foo1" }, cts);
        await fooRepository.AddAsync(new Foo() { Text = "Foo2" }, cts);
        await barRepository.AddAsync(new Bar() { Text = "Bar1" }, cts);

        await uow.SaveAsync(cts);
    }
}
```

Within the NuGet package, there are more interfaces along with their corresponding base implementations.
- [AsyncReadRepositoryBase<TEntity>](./AsyncReadRepositoryBase.cs): Base class for asynchronous read repositories. It implements [IAsyncReadRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IAsyncReadRepository.cs).
- [AsyncRepositoryBase<TEntity>](./AsyncRepositoryBase.cs): Base class for asynchronous repositories. It implements [IAsyncRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IAsyncRepository.cs).
- [AsyncUnitOfWorkBase](./AsyncUnitOfWorkBase.cs): Base class for an asynchronous unit of work. It implements [IAsyncUnitOfWork](../Encamina.Enmarcha.Data.Abstractions/IAsyncUnitOfWork.cs).
- [AsyncWriteRepositoryBase<TEntity>](./AsyncWriteRepositoryBase.cs): Base class for asynchronous write repositories. It implements [IAsyncWriteRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IAsyncWriteRepository.cs).
- [FullRepositoryBase<TEntity>](./FullRepositoryBase.cs): Base class for a repository with both synchronous and asynchronous operations to read and write entities. It implements [IFullRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IFullRepository.cs).
- [FullUnitOfWork](./FullUnitOfWork.cs): Base class for a unit of work that can manage both synchronous and asynchronous operations. It implements [IFullUnitOfWork](../Encamina.Enmarcha.Data.Abstractions/IFullUnitOfWork.cs).
- [ReadRepositoryBase<TEntity>](./ReadRepositoryBase.cs): Base class for a read repositories. It implements [IReadRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IReadRepository.cs).
- [RepositoryBase<TEntity>](./RepositoryBase.cs): Base class for a repositories. It implements [IRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IRepository.cs).
- [UnitOfWorkBase](./UnitOfWorkBase.cs): Base class for an unit of work. It implements [IUnitOfWork](../Encamina.Enmarcha.Data.Abstractions/IUnitOfWork.cs).
- [WriteRepositoryBase<TEntity>](./WriteRepositoryBase.cs): Base class for write repositories. It implements [IWriteRepository<TEntity>](../Encamina.Enmarcha.Data.Abstractions/IWriteRepository.cs).

### Other functionalities

Inside the NuGet package, you will find some extension methods related to Entity Framework.
```csharp
// ...

var queryableFoo = dbContext.AsQueryable<Foo>(withNoTracking: true, withIdentityResolution: true);

// More methods available in DbContextExtensions...
```