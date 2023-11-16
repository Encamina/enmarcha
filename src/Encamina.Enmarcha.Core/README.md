# Core

Core is a project that primarily contains cross-cutting utilities that can be used in a wide variety of projects. Among them, there are utilities related to Uris, Strings, Attributes, JSON handling, and more.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Core](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Core

### .NET CLI:

First, [install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Then, install [Encamina.Enmarcha.Core](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Core

## How to use
Below are some of the most important utilities. It is not a complete list of all functionalities.

### JsonUtils

[JsonUtils](./Extensions/JsonUtils.cs) contains a collection of static methods that provide utilities for JSON handling.

```csharp
string json = "{\"Name\":\"John\",\"Age\":30,\"City\":\"Valencia\"}";

var deserializedObject = JsonUtils.DeserializeAnonymousType(json, new { Name = string.Empty, Age = 0, City = string.Empty });
```
The above code deserializes a string into an anonymous type, eliminating the need to create a class.

### SdkVersionUtils
[SdkVersionUtils](./Extensions/SdkVersionUtils.cs) is an utility class for the version information of the current assembly.

```csharp
var version = SdkVersionUtils.GetSdkVersion("Program SDK Version: ", Assembly.GetAssembly(typeof(Program)));
// Program SDK Version: 6.0.3-16
```

### UriExtensions
[UriExtensions](./Extensions/UriExtensions.cs) is an extension methods class for handling `Uris`.

```csharp
var uri = new Uri("https://www.enmarcha.net/");

var newUri = uri.Append("acme", "products");
// newUri => https://www.enmarcha.net/acme/products
```

### StringExtensions
[StringExtensions](./Extensions/StringExtensions.cs) is an extension methods class for handling `strings`.

```csharp
var email = "enmarcha@email.com";

var isValidEmail = email.IsValidEmail();
// true
```

### ObjectExtensions
[ObjectExtensions](./Extensions/ObjectExtensions.cs) is an extension methods class for handling `objects`.

```csharp
var data = new { Name = "Siul", Age = 43, City = "Adarrefnop" };

var dictionary = data.ToPropertyDictionary();
// {Name, Siul}
// {Age, 43}
// {City, Adarrefnop}
```

### IListExtensions
[IListExtensions](./Extensions/IListExtensions.cs) is an extension methods class for handling `IList<T>`.

```csharp
IList<string> values = GetValues(); // Foo, Bar
var newNewValues = new[] { "FooBar", "BarFoo" };

values.AddRange(newNewValues);
// values =>  Foo, Bar, FooBar, BarFoo
```

### IDictionaryExtensions
[IDictionaryExtensions](./Extensions/IDictionaryExtensions.cs) is an extension methods class for handling `IDictionary<TKey, TValue>`.

```csharp
IDictionary<int, string> dictionary1 = new Dictionary<int, string>() { { 1, "one" }, { 2, "two" }, };

IDictionary<int, string> dictionary2 = new Dictionary<int, string>() { { 3, "three" }, { 4, "four" }, };

dictionary1.Merge(dictionary2);
// {1, one}
// {2, two}
// {3, three}
// {4, four}
```

### ICollectionExtensions
[ICollectionExtensions](./Extensions/ICollectionExtensions.cs) is an extension methods class for handling `ICollection<T>`.

```csharp
ICollection<string> values = GetValues(); // Foo, Bar
var newNewValues = new[] { "FooBar", "BarFoo" };

values.AddRange(newNewValues);
// values =>  Foo, Bar, FooBar, BarFoo
```

### EnumExtensions
[EnumExtensions](./Extensions/EnumExtensions.cs) is an extension methods class for handling `Enum` types.

```csharp
public enum TransportType
{
    [Description("Transport by Airplane")]
    Airplane,

    [Description("Transport by Car")]
    Car,

    [Description("Transport by Train")]
    Train,

    [Description("Transport by Bus")]
    Bus,

    [Description("Transport by Bicycle")]
    Bicycle
}

// ...

var currentTransport = TransportType.Airplane;

var airplaneDescription = currentTransport.GetEnumDescription();
// airplaneDescription => Transport by Airplane
```

### CultureInfoExtensions
[CultureInfoExtensions](./Extensions/CultureInfoExtensions.cs) is an extension methods class for handling `CultureInfo`.

```csharp
var currentCulture = new CultureInfo("es-MX");
var newCulture = new CultureInfo("es");

bool matches = currentCulture.MatchesWith(newCulture);
// matches => true
```

### UriAttribute
[UriAttribute](./DataAnnotations/UriAttribute.cs) provides 'Uri' validation.

```csharp
public class MyClass
{
    [Uri(AllowsNull = false)]
    public Uri Uri { get; set; }
}
```

There are more extension methods, classes with extension methods, attributes, among others, available.