# Enmarcha Aspire

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Aspire)](https://www.nuget.org/packages/Encamina.Enmarcha.Aspire)

Aspire Resource Builder Extensions is a set of tools designed to simplify the configuration of cloud-native resources with environment variables. This package is ideal for projects that need to manage distributed application resources in a cloud environment using environment-specific configurations.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Aspire](https://www.nuget.org/packages/Encamina.Enmarcha.Aspire) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Aspire

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Aspire](https://www.nuget.org/packages/Encamina.Enmarcha.Aspire) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Aspire

## How to use

### Resource Builder Extensions

The `ResourceBuilderExtensions` class provides a set of extension methods to configure Aspire resources.

```csharp
var agentProjects = builder.AddProject<Projects.AgentProjects>(appIdProjects)
                           .WithEnvironment(Constants.Settings.Options.PendingTaskStatuses, pendingTaskStatuses)
```
