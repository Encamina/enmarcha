# Testing - SMPT

[![Nuget package](https://img.shields.io/nuget/v/Encamina.Enmarcha.Testing.Smtp)](https://www.nuget.org/packages/Encamina.Enmarcha.Testing.Smtp)

This project provides utilities for tests related to SMTP servers, allowing you to perform unit tests simulating all the aspects related to SMTP.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Testing.Smtp](https://www.nuget.org/packages/Encamina.Enmarcha.Testing.Smtp) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Testing.Smtp

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Testing.Smtp](https://www.nuget.org/packages/Encamina.Enmarcha.Testing.Smtp) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Testing.Smtp

## How to use

At some initial point in your tests, for example, in the constructor of a test class, you can initialize a new instance of the SMTP server.
```csharp
public class EmailServiceTests : FakerProviderFixturedBase, IDisposable
{
    private SmtpServer smtpServer;

    public EmailServiceTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
        smtpServer = Configuration.Configure()
                                  .WithRandomPort()
                                  .Build();
    }

    public void Dispose()
    {
        smtpServer.Dispose();
        smtpServer = null;
    }
}
```

With this, you will achieve having the basic functionalities of an SMTP server locally to perform tests on an email service without external dependencies.

```csharp
[Fact]
public void EmailService_SmtpClientOptions_ValidOptions_WithCustomName_Succeeds()
{
    // Arrange
    var smtpClientOptions = new SmtpClientOptions()
    {
        Host = smtpServer.Configuration.Domain,
        Port = smtpServer.Configuration.Port,
        Name = "dummy name",
        Password = "dummy password",
        User = "dummy user",
        // For test porposes, SSL should not be used. Otherwise, a `MailKit.Security.SslHandshakeException` might be thrown...
        UseSSL = false,
    };

    var emailService = new EmailService(smtpClientOptions);

    // ....

    // Act...
    emailService.SendAsync();

    // Assert
    // ...
}
```