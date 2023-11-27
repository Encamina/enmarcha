# Email - MailKit

MailKit is a project designed to simplify the configuration and sending of emails. The most typical use case is configuring your SMTP account and sending an email with attachments, subject, and more.

## Setup

### Nuget package

First, [install NuGet](http://docs.nuget.org/docs/start-here/installing-nuget). Then, install [Encamina.Enmarcha.Email.MailKit](https://www.nuget.org/packages/Encamina.Enmarcha.Email.MailKit) from the package manager console:

    PM> Install-Package Encamina.Enmarcha.Email.MailKit

### .NET CLI:
[Install .NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/). Next, install [Encamina.Enmarcha.Email.MailKit](https://www.nuget.org/packages/Encamina.Enmarcha.Email.MailKit) from the .NET CLI:

    dotnet add package Encamina.Enmarcha.Email.MailKit

## How to use

First, you need to add the [SmtpClientOptions](../Encamina.Enmarcha.Email.Abstractions/SmtpClientOptions.cs) to your project configuration. You can achieve this by using any [configuration provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration). The followng code is an example of how the settings should look like using the `appsettings.json` file:

```json
  {
    // ...
    "SmtpClientOptions": {
        "User": "<SMPT_USER>", // User credential required to connect to the SMTP service
        "Password": "<SMPT_PASSWORD>", // Password credential required to connect to the SMTP service
        "Port": "<SMPT_PORT>", // Port for the SMTP service
        "Host": "<SMPT_HOST>" // Host name of the SMTP service.
    }
    // ...
  }
```

Next, in `Program.cs` or a similar entry point file in your project, add the following code.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

// Or others configuration providers...
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 

builder.Services.AddMailKitEmailProvider(builder.Configuration, serviceLifetime: ServiceLifetime.Singleton);
```

There are also other overloads of `AddMailKitEmailProvider` on which you can directly specify the SMTP data.

```csharp
// Entry point
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
   // ...
});

// ...

builder.Services.AddMailKitEmailProvider(options =>
{
    options.User = "<SMPT_USER>"; // User credential required to connect to the SMTP service
    options.Password = "<SMPT_PASSWORD>"; // Password credential required to connect to the SMTP service
    options.Port = "<SMPT_PORT>"; // Port for the SMTP service
    options.Host = "<SMPT_HOST>"; // Host name of the SMTP service.
});
```

With this, we can resolve the `IEmailProvider` interface and be able to send an email.

```csharp
public class MyClass
{
    private readonly IEmailProvider emailProvider;

    public MyClass(IEmailProvider emailProvider)
    {
        this.emailProvider = emailProvider;
    }

    public async Task SendTestEmailAsync(CancellationToken cancellationToken)
    {
        await emailProvider.BeginSendEmail()
            .SetDefaultSender()
            .SetSubject("Test email Subject")
            .SetBody("Test body subject")
            .AddRecipient("contoso@email.com")
            .AddAttachment("testFile.txt", Encoding.UTF8.GetBytes("Attachment File"))
            .SendAsync(cancellationToken);
    }
}
```