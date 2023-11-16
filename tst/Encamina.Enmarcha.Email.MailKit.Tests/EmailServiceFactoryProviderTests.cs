using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Entities.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Encamina.Enmarcha.Email.MailKit.Tests;

public sealed class EmailServiceFactoryProviderTests
{
    [Fact]
    public void EmailServiceFactoryProvider_CreateWithNullServiceScopeFactory_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new EmailServiceFactoryProvider(null));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"serviceScopeFactory", argumentNullException.ParamName);
    }

    [Fact]
    public void EmailServiceFactoryProvider_CreateServiceScopeFactory_Succeeds()
    {
        // Arrange...
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var testServiceScopeFactory = mockServiceScopeFactory.Object;

        // Act...
        var emailServiceFactoryProvider = new EmailServiceFactoryProvider(testServiceScopeFactory);

        // Assert...
        Assert.NotNull(emailServiceFactoryProvider);
        Assert.NotNull(emailServiceFactoryProvider.ServiceScopeFactory);
        Assert.Same(testServiceScopeFactory, emailServiceFactoryProvider.ServiceScopeFactory);
    }

    [Fact]
    public void EmailServiceFactoryProvider_CreateWithNullServiceFactoryBuilder_ThrowsException()
    {
        // Arrange...
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var testServiceScopeFactory = mockServiceScopeFactory.Object;

        // Act...
        var exception = Record.Exception(() => new EmailServiceFactoryProvider(testServiceScopeFactory, null));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(@"serviceFactoryBuilder", argumentNullException.ParamName);
    }

    [Fact]
    public void EmailServiceFactoryProvider_CreateServiceScopeFactoryAndServiceFactoryBuilder_Succeeds()
    {
        // Arrange...
        var mockServiceScopeFactory = new Mock<IServiceScopeFactory>();
        var testServiceScopeFactory = mockServiceScopeFactory.Object;

        var mockServiceFactory = new Mock<IServiceFactory<IEmailProvider>>();
        var testServiceFactory = mockServiceFactory.Object;

        // Act...
        var emailServiceFactoryProvider = new EmailServiceFactoryProvider(testServiceScopeFactory, ssp => testServiceFactory);
        var serviceFactory = emailServiceFactoryProvider.GetScopedServiceFactory();

        // Assert...
        Assert.NotNull(emailServiceFactoryProvider);
        Assert.NotNull(emailServiceFactoryProvider.ServiceScopeFactory);
        Assert.Same(testServiceScopeFactory, emailServiceFactoryProvider.ServiceScopeFactory);
        Assert.Same(serviceFactory, testServiceFactory);
    }
}
