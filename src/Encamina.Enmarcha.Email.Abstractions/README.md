# Email - Abstractions

This project mainly contains abstractions used by the Encamina.Enmarcha.Email.MailKit NuGet package. Essentially, it includes interfaces and entities related to email sending management.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Email.Abstractions](ToDo:NugetUrl) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Email.Abstractions

### .NET CLI:

[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Email.Abstractions](ToDo:NugetUrl) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Email.Abstractions

## How to use

Typically, is not necessary an explicit use of this project unless you are creating your own implementation of one of the interfaces. However, the most important elements are as follows:
- [IEmailProvider](./IEmailProvider.cs) represents a provider to build and send e-mails. It has its implementation as [EmailService](../Encamina.Enmarcha.Email.MailKit/EmailService.cs) in Encamina.Enmarcha.Email.MailKit NuGet package.
- [IEmailProviderFactory](./IEmailProviderFactory.cs) represents a factory that can provide valid instances of a specific `IEmailProvider` type. It has its implementation in as [EmailServiceFactory](../Encamina.Enmarcha.Email.MailKit/EmailServiceFactory.cs) in Encamina.Enmarcha.Email.MailKit NuGet package.
- [IEmailProviderFactoryProvider](./IEmailProviderFactoryProvider.cs) represents a provider for factories of `IEmailProvider`. It has its implementation as [EmailServiceFactoryProvider](../Encamina.Enmarcha.Email.MailKit/EmailServiceFactoryProvider.cs) in Encamina.Enmarcha.Email.MailKit NuGet package.

In addition to these interfaces, there are numerous classes such as `EmailSpecification`, `EmailAttachmentSpecification`, or `EmailAddressSpecification` that define the specifications for different configurations related to email sending. Previously, it is necessary to create a specific implementation for email sending, using the Encamina.Enmarcha.Email.MailKit NuGet package (see [documentation](../Encamina.Enmarcha.Email.MailKit/README.md)). It provides fully transparency.