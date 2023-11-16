using Bogus;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Testing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Email.MailKit.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class IServiceCollectionExtensionsTests : FakerProviderFixturedBase
{
    public IServiceCollectionExtensionsTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void AddMailKitEmailProvider_CalledMultipleTimes_DifferentConfigurations_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testOptionsScoped = BuildValidSmtpClientOptions(faker);
        var testOptionsSingleton = BuildValidSmtpClientOptions(faker);
        var testOptionsTransient = BuildValidSmtpClientOptions(faker);

        var testServiceProvider = new ServiceCollection()
            .AddMailKitEmailProvider(options =>
            {
                options.Port = testOptionsScoped.Port;
                options.Host = testOptionsScoped.Host;
                options.Name = testOptionsScoped.Name;
                options.Password = testOptionsScoped.Password;
                options.User = testOptionsScoped.User;
                options.UseSSL = testOptionsScoped.UseSSL;
            }, ServiceLifetime.Scoped)
            .AddMailKitEmailProvider(options =>
            {
                options.Port = testOptionsSingleton.Port;
                options.Host = testOptionsSingleton.Host;
                options.Name = testOptionsSingleton.Name;
                options.Password = testOptionsSingleton.Password;
                options.User = testOptionsSingleton.User;
                options.UseSSL = testOptionsSingleton.UseSSL;
            }, ServiceLifetime.Singleton)
            .AddMailKitEmailProvider(options =>
            {
                options.Port = testOptionsTransient.Port;
                options.Host = testOptionsTransient.Host;
                options.Name = testOptionsTransient.Name;
                options.Password = testOptionsTransient.Password;
                options.User = testOptionsTransient.User;
                options.UseSSL = testOptionsTransient.UseSSL;
            }, ServiceLifetime.Transient)
            .BuildServiceProvider();

        // Act...
        var services = testServiceProvider.GetServices<IEmailProvider>();
        var factories = testServiceProvider.GetServices<IEmailProviderFactory>();
        var factoryProviders = testServiceProvider.GetServices<IEmailProviderFactoryProvider>();

        // Assert...
        Assert.NotNull(services);
        Assert.NotEmpty(services);
        Assert.True(services.Count() == 3); // Assert there are as many services as they have been registered...

        Assert.NotNull(factories);
        Assert.Empty(factories); // Assert there are no service factories, since any factory should be obtained from the factory provider...

        Assert.NotNull(factoryProviders);
        Assert.NotEmpty(factoryProviders);
        var factoryProvider = Assert.Single(factoryProviders); // Assert there is only one factory...

        using var factory = factoryProvider.GetScopedServiceFactory();

        // Assert that calling twice for the "Scoped" service instance returns the same instance...
        Assert.Same(factory.ByName(testOptionsScoped.Name), factory.ByName(testOptionsScoped.Name));

        // Assert that calling twice for the "Singleton" service instance returns the same instance...
        Assert.Same(factory.ByName(testOptionsSingleton.Name), factory.ByName(testOptionsSingleton.Name));

        // Assert that calling twice for the "Transient" service instance returns the different instances...
        Assert.NotSame(factory.ByName(testOptionsTransient.Name), factory.ByName(testOptionsTransient.Name));

        // Assert that calling twice for the factory provider returns the same instance since it should be registered as "Singleton"...
        Assert.Same(factoryProvider, testServiceProvider.GetService<IEmailProviderFactoryProvider>());
    }

    [Fact]
    public void AddMailKitEmailProvider_JustOneCall_UsingValidConfiguration_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var result = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(testOptions.Name, result.Name);
    }

    [Fact]
    public void AddMailKitEmailProvider_JustOneCall_UsingValidOptions_Succeeds()
    {
        var testOptions = BuildValidSmtpClientOptions(FakerProvider.GetFaker());

        // Arrange...
        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var result = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(testOptions.Name, result.Name);
    }

    [Fact]
    public void AddMailKitEmailProvider_UsingConfiguration_AsNull_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new ServiceCollection().AddMailKitEmailProvider(configuration: null));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"config", argumentNullException.ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddMailKitEmailProvider_UsingConfiguration_WithInvalidPort_ThrowsException(bool useNegative)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);
        testOptions.Port = useNegative ? faker.Random.Int(max: 0) : faker.Random.Int(min: 65535);

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        var optionsValidationException = Assert.IsType<OptionsValidationException>(exception);

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Port), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfiguration_WithNullOrEmptyOrWhiteSpaceHost_ThrowsException(string invalidHost)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);
        testOptions.Host = invalidHost;

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        var optionsValidationException = Assert.IsType<OptionsValidationException>(exception);

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Host), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfiguration_WithNullOrEmptyOrWhiteSpaceName_NameIsHostAndPort_Succeds(string name)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);
        testOptions.Name = name;

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        var service = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(service);
        var smtpClientOptionsProvider = Assert.IsAssignableFrom<ISmtpClientOptionsProvider>(service);

        Assert.NotNull(smtpClientOptionsProvider.SmtpClientOptions);
        Assert.Equal($@"{testOptions.Host}:{testOptions.Port}", smtpClientOptionsProvider.SmtpClientOptions.Name);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfiguration_WithNullOrEmptyOrWhiteSpacePassword_ThrowsException(string invalidPassword)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);
        testOptions.Password = invalidPassword;

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        var optionsValidationException = Assert.IsType<OptionsValidationException>(exception);

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Password), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfiguration_WithNullOrEmptyOrWhiteSpaceUser_ThrowsException(string invalidUser)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);
        testOptions.User = invalidUser;

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build()).BuildServiceProvider();

        // Act...
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        var optionsValidationException = Assert.IsType<OptionsValidationException>(exception);

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.User), optionsValidationException.Message);
    }

    [Fact]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithConfigurationAsNull_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new ServiceCollection().AddMailKitEmailProvider(null, _ => { }));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"config", ((ArgumentNullException)exception).ParamName);
    }

    [Fact]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithInvalidConfiguration_WithValidOptions_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        });

        var allWrongOptions = new SmtpClientOptions()
        {
            Host = faker.Random.Word(),
            Name = faker.Random.Word(),
            Password = faker.Random.Word(),
            Port = faker.Random.Int(max: 0),
            User = null,
        };

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(allWrongOptions.ToPropertyDictionary()).Build(), testAction).BuildServiceProvider();

        // Act...
        var result = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(testOptions.Name, result.Name);
    }

    [Fact]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithOptionsAsNull_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().Build(), null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"configureOptions", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithValidConfiguration_InOptions_WithInvalidPort_ThrowsException(bool useNegative)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testOptionsAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = useNegative ? faker.Random.Int(max: 0) : faker.Random.Int(min: 65535);
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        });

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build(), testOptionsAction).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Port), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithValidConfiguration_InOptions_WithNullOrEmptyOrWhiteSpaceHost_ThrowsException(string invalidHost)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testOptionsAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = invalidHost;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        });

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build(), testOptionsAction).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Host), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithValidConfiguration_InOptions_WithNullOrEmptyOrWhiteSpaceName_NameIsHostAndPort_Succeds(string name)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testOptionsAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = testOptions.Host;
            options.Name = name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        });

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build(), testOptionsAction).BuildServiceProvider();

        // Act...
        var service = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(service);
        var smtpClientOptionsProvider = Assert.IsAssignableFrom<ISmtpClientOptionsProvider>(service);

        Assert.NotNull(smtpClientOptionsProvider.SmtpClientOptions);
        Assert.Equal($@"{testOptions.Host}:{testOptions.Port}", smtpClientOptionsProvider.SmtpClientOptions.Name);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithValidConfiguration_InOptions_WithNullOrEmptyOrWhiteSpacePassword_ThrowsException(string invalidPassword)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testOptionsAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = invalidPassword;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        });

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build(), testOptionsAction).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Password), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_UsingConfigurationAndOptions_WithValidConfiguration_InOptions_WithNullOrEmptyOrWhiteSpaceUser_ThrowsException(string invalidUser)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testOptions = BuildValidSmtpClientOptions(faker);

        var testOptionsAction = new Action<SmtpClientOptions>(options =>
        {
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = invalidUser;
            options.UseSSL = testOptions.UseSSL;
        });

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(new ConfigurationBuilder().AddInMemoryCollection(testOptions.ToPropertyDictionary()).Build(), testOptionsAction).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.User), optionsValidationException.Message);
    }

    [Fact]
    public void AddMailKitEmailProvider_WithOptions_AsNull_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new ServiceCollection().AddMailKitEmailProvider(options: null));

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"configureOptions", ((ArgumentNullException)exception).ParamName);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AddMailKitEmailProvider_WithOptions_WithInvalidPort_ThrowsException(bool useNegative)
    {
        // Arrange...
        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            var faker = FakerProvider.GetFaker();
            var testOptions = BuildValidSmtpClientOptions(faker);
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = useNegative ? faker.Random.Int(max: 0) : faker.Random.Int(min: 65535);
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Port), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_WithOptions_WithNullOrEmptyOrWhiteSpaceHost_ThrowsException(string invalidHost)
    {
        // Arrange...
        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            var faker = FakerProvider.GetFaker();
            var testOptions = BuildValidSmtpClientOptions(faker);
            options.Host = invalidHost;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Host), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_WithOptions_WithNullOrEmptyOrWhiteSpaceName_NameIsHostAndPort_Succeds(string name)
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();
        var testOptions = BuildValidSmtpClientOptions(faker);

        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            options.Host = testOptions.Host;
            options.Name = name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        var service = testServiceProvider.GetRequiredService<IEmailProvider>();

        // Assert...
        Assert.NotNull(service);
        var smtpClientOptionsProvider = Assert.IsAssignableFrom<ISmtpClientOptionsProvider>(service);

        Assert.NotNull(smtpClientOptionsProvider.SmtpClientOptions);
        Assert.Equal($@"{testOptions.Host}:{testOptions.Port}", smtpClientOptionsProvider.SmtpClientOptions.Name);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_WithOptions_WithNullOrEmptyOrWhiteSpacePassword_ThrowsException(string invalidPassword)
    {
        // Arrange...
        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            var faker = FakerProvider.GetFaker();
            var testOptions = BuildValidSmtpClientOptions(faker);
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = invalidPassword;
            options.Port = testOptions.Port;
            options.User = testOptions.User;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.Password), optionsValidationException.Message);
    }

    [Theory]
    [InlineData(@"")]
    [InlineData(@" ")]
    [InlineData(null)]
    public void AddMailKitEmailProvider_WithOptions_WithNullOrEmptyOrWhiteSpaceUser_ThrowsException(string invalidUser)
    {
        // Arrange...
        var testServiceProvider = new ServiceCollection().AddMailKitEmailProvider(options =>
        {
            var faker = FakerProvider.GetFaker();
            var testOptions = BuildValidSmtpClientOptions(faker);
            options.Host = testOptions.Host;
            options.Name = testOptions.Name;
            options.Password = testOptions.Password;
            options.Port = testOptions.Port;
            options.User = invalidUser;
            options.UseSSL = testOptions.UseSSL;
        }).BuildServiceProvider();

        // Act...
        // Options validations usually occurs when the service type is required
        var exception = Record.Exception(testServiceProvider.GetRequiredService<IEmailProvider>);

        // Assert...
        Assert.NotNull(exception);
        Assert.IsType<OptionsValidationException>(exception);

        var optionsValidationException = (OptionsValidationException)exception;

        // The message in a `OptionsValidationException` could be quite complex. Just assert it contains (or refers) to the right property.
        Assert.Single(optionsValidationException.Failures);
        Assert.Contains(nameof(SmtpClientOptions.User), optionsValidationException.Message);
    }

    private static SmtpClientOptions BuildValidSmtpClientOptions(Faker faker)
    {
        return new SmtpClientOptions()
        {
            Host = faker.Internet.DomainName(),
            Port = faker.Internet.Port(),
            Password = faker.Internet.Password(),
            User = faker.Internet.Email(),
            UseSSL = faker.Random.Bool(),
        };
    }
}
