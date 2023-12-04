# Data - Abstractions

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Data.Abstractions)](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Abstractions)

This project mainly contains abstractions related to data used by other ENMARCHA NuGet packages or necessary for creating your own implementations.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Data.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Abstractions) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Data.Abstractions

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Data.Abstractions](https://www.nuget.org/packages/Encamina.Enmarcha.Data.Abstractions) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Data.Abstractions

## How to use

In addition to the abstractions (interfaces, abstract classes, etc.) that have their implementations in other ENMARCHA NuGets, such as [IFullUnitOfWork](./IFullUnitOfWork.cs)/[FullUnitOfWork](../Encamina.Enmarcha.Data.EntityFramework/FullUnitOfWork.cs), you can create your own implementations.
 
```csharp
public class InMemoryReadRepository<TEntity> : IReadRepository<TEntity> 
    where TEntity : IIdentifiable // IIdentifiable requires Encamina.Enmarcha.Entities.Abstractions nuget
{
    private readonly List<TEntity> entities = new();

    public IQueryable<TEntity> GetAll()
    {
        return entities.AsQueryable();
    }

    public IQueryable<TEntity> GetAll([NotNull] Func<IQueryable<TEntity>, IQueryable<TEntity>> queryFunction)
    {
        return entities.AsQueryable();
    }

    public TEntity GetById<TEntityId>(TEntityId id)
    {
        var entity = entities.FirstOrDefault(e => e.Id.Equals(id));
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with ID {id} not found.");
        }
        return entity;
    }
} 
```
> This is example code, it is not tested. Do not use it in production.

In the previous example, an in-memory implementation of `IReadRepository<TEntity>` is created.