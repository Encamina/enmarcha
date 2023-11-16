using Encamina.Enmarcha.Email.Abstractions;
using Encamina.Enmarcha.Testing;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Encamina.Enmarcha.Email.MailKit.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class EmailServiceFactoryTests : FakerProviderFixturedBase
{
    public EmailServiceFactoryTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void EmailServiceFactory_NullServiceProvider_ThrowsException()
    {
        // Act...
        var exception = Record.Exception(() => new EmailServiceFactory(null));

        // Assert...
        Assert.NotNull(exception);
        var argumentNullExcetion = Assert.IsType<ArgumentNullException>(exception);

        Assert.Equal(@"serviceScope", argumentNullExcetion.ParamName);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromEmptyServiceProvider_ThrowsException()
    {
        // Arrange...
        var testKey = FakerProvider.GetFaker().Random.Word();

        // Act...
        var exception = Record.Exception(() => new EmailServiceFactory(new ServiceCollection().BuildServiceProvider().CreateScope()).ByName(testKey));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);

        Assert.Equal(@"serviceName", argumentException.ParamName);
        Assert.Equal($@"There isn't a service of type '{typeof(IEmailProvider)}' configured with name '{testKey}'. Please check the configuration! (Parameter 'serviceName')", argumentException.Message);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromEmptyServiceProvider_WithFlagToThrowExceptionIfNotFound_AsTrue_ThrowsException()
    {
        // Arrange...
        var testKey = FakerProvider.GetFaker().Random.Word();

        // Act...
        var exception = Record.Exception(() => new EmailServiceFactory(new ServiceCollection().BuildServiceProvider().CreateScope()).ByName(testKey, true));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);

        Assert.Equal(@"serviceName", argumentException.ParamName);
        Assert.Equal($@"There isn't a service of type '{typeof(IEmailProvider)}' configured with name '{testKey}'. Please check the configuration! (Parameter 'serviceName')", argumentException.Message);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromEmptyServiceProvider_WithFlagToThrowExceptionIfNotFound_AsFalse_Succeeds()
    {
        // Arrange...
        var testKey = FakerProvider.GetFaker().Random.Word();

        // Act...
        var result = new EmailServiceFactory(new ServiceCollection().BuildServiceProvider().CreateScope()).ByName(testKey, false);

        // Assert...
        Assert.Null(result);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromValidServiceProvider_WithNonExistentName_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var mockEmailProvider = new Mock<IEmailProvider>();
        mockEmailProvider.SetupGet(s => s.Name).Returns(faker.Random.Word());

        var testKey = faker.Random.Word();
        var testEmailServiceFactory = new EmailServiceFactory(new ServiceCollection().AddTransient(_ => mockEmailProvider.Object).BuildServiceProvider().CreateScope());

        // Act...
        var exception = Record.Exception(() => testEmailServiceFactory.ByName(testKey));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);

        Assert.Equal(@"serviceName", argumentException.ParamName);
        Assert.Equal($@"There isn't a service of type '{typeof(IEmailProvider)}' configured with name '{testKey}'. Please check the configuration! (Parameter 'serviceName')", argumentException.Message);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromValidServiceProvider_WithNonExistentName_WithFlagToThrowExceptionIfNotFound_AsTrue_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var mockEmailProvider = new Mock<IEmailProvider>();
        mockEmailProvider.SetupGet(s => s.Name).Returns(faker.Random.Word());

        var testKey = faker.Random.Word();
        var testEmailServiceFactory = new EmailServiceFactory(new ServiceCollection().AddTransient(_ => mockEmailProvider.Object).BuildServiceProvider().CreateScope());

        // Act...
        var exception = Record.Exception(() => testEmailServiceFactory.ByName(testKey, true));

        // Assert...
        Assert.NotNull(exception);
        var argumentException = Assert.IsType<ArgumentException>(exception);

        Assert.Equal(@"serviceName", argumentException.ParamName);
        Assert.Equal($@"There isn't a service of type '{typeof(IEmailProvider)}' configured with name '{testKey}'. Please check the configuration! (Parameter 'serviceName')", argumentException.Message);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromValidServiceProvider_WithNonExistentName_WithFlagToThrowExceptionIfNotFound_AsFalse_ThrowsException()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var mockEmailProvider = new Mock<IEmailProvider>();
        mockEmailProvider.SetupGet(s => s.Name).Returns(faker.Random.Word());

        var testKey = faker.Random.Word();
        var testEmailServiceFactory = new EmailServiceFactory(new ServiceCollection().AddTransient(_ => mockEmailProvider.Object).BuildServiceProvider().CreateScope());

        // Act...
        var result = testEmailServiceFactory.ByName(testKey, false);

        // Assert...
        Assert.Null(result);
    }

    [Fact]
    public void EmailServiceFactory_GetByName_FromValidServiceProvider_WithValidName_Succeeds()
    {
        // Arrange...
        var testKey = FakerProvider.GetFaker().Random.Word();

        var mockEmailProvider = new Mock<IEmailProvider>();
        mockEmailProvider.SetupGet(s => s.Name).Returns(testKey);

        var testEmailProvider = mockEmailProvider.Object;

        var testServiceProvider = new ServiceCollection().AddTransient(_ => testEmailProvider)
                                                         .AddTransient(testEmailProvider.GetType(), _ => testEmailProvider)
                                                         .BuildServiceProvider();

        // Act...
        var result = new EmailServiceFactory(testServiceProvider.CreateScope()).ByName(testKey);

        // Assert...
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEmailProvider>(result);
        Assert.Equal(testKey, result.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EmailServiceFactory_GetByName_FromValidServiceProvider_WithValidName_WithFlagToThrowExceptionIfNotFound_Succeeds(bool throwIfNotFound)
    {
        // Arrange...
        var testKey = FakerProvider.GetFaker().Random.Word();

        var mockEmailProvider = new Mock<IEmailProvider>();
        mockEmailProvider.SetupGet(s => s.Name).Returns(testKey);

        var testEmailProvider = mockEmailProvider.Object;

        var testServiceProvider = new ServiceCollection().AddTransient(_ => testEmailProvider)
                                                         .AddTransient(testEmailProvider.GetType(), _ => testEmailProvider)
                                                         .BuildServiceProvider();

        // Act...
        var result = new EmailServiceFactory(testServiceProvider.CreateScope()).ByName(testKey, throwIfNotFound);

        // Assert...
        Assert.NotNull(result);
        Assert.Equal(testKey, result.Name);
    }
}
