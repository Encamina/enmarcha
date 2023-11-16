# Testing

This project provides utilities to facilitate testing.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Testing](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Testing

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Testing](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Testing

## How to use

### FakeProvider

[FakeProvider](./FakerProvider.cs) provides a faker component that replace external dependencies or values in-place to run tests with an expect or particular outcome. This faker provided is powered by [Bogus](https://github.com/bchavez/Bogus).
Although you can instantiate a `FakeProvider` in your tests, if you are using xUnit, the simplest approach is to use a `Collection Fixture` and share the `FakeProvider` across your different test classes.  For this purpose, the auxiliary class [FakerProviderFixturedBase](./FakerProviderFixturedBase.cs) is provided, which implements the [IFakerProviderFixture](./IFakerProviderFixture). Here is a simple example:

```csharp
[CollectionDefinition(MagicStrings.FixturesCollection)]
public class FixturesCollection : ICollectionFixture<FakerProvider>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection(MagicStrings.FixturesCollection)]
public class TestClass1 : FakerProviderFixturedBase
{
    public TestClass1(FakerProvider fakerFixture) : base(fakerFixture)
    {        
    }

    [Fact]
    public void DummyTest()
    {
        // ...
        var faker = FakerProvider.GetFaker();
        // faker.Address

        // ...
    }
}

[Collection(MagicStrings.FixturesCollection)]
public class TestClass2 : FakerProviderFixturedBase
{
    public TestClass2(FakerProvider fakerFixture) : base(fakerFixture)
    {        
    }

     [Fact]
    public void OtherDummyTest()
    {
        // ...
        var faker = FakerProvider.GetFaker();
        // faker.Company

        // ...
    }
}
```