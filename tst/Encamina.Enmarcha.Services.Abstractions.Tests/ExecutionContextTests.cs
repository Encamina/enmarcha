using System.Globalization;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Testing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

namespace Encamina.Enmarcha.Services.Abstractions.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class ExecutionContextTests : FakerProviderFixturedBase
{
    public ExecutionContextTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void ExecutionContext_Succeeds()
    {
        // Act...
        var executionContext = new ExecutionContext();

        // Assert...
        Assert.NotNull(executionContext);
        Assert.NotNull(executionContext.CorrelationCallId);
        Assert.NotNull(executionContext.CorrelationId);

        Assert.Null(executionContext.Configuration);

        Assert.Equal(CultureInfo.InvariantCulture, executionContext.CultureInfo);
        Assert.Equal(CancellationToken.None, executionContext.CancellationToken);

        Assert.NotEqual(Guid.Empty, executionContext.Id);
    }

    [Fact]
    public void ExecutionContext_FromEmptyTemplate_Succeeds()
    {
        // Arrange...
        var testExecutionContextTemplate = new ExecutionContextTemplate();

        // Act...
        var executionContext = new ExecutionContext(testExecutionContextTemplate);

        // Assert...
        Assert.NotNull(executionContext);

        Assert.Equal(testExecutionContextTemplate.CorrelationCallId, executionContext.CorrelationCallId);
        Assert.Equal(testExecutionContextTemplate.CorrelationId, executionContext.CorrelationId);
        Assert.Equal(testExecutionContextTemplate.Configuration, executionContext.Configuration);
        Assert.Equal(testExecutionContextTemplate.CultureInfo, executionContext.CultureInfo);
        Assert.Equal(testExecutionContextTemplate.CancellationToken, executionContext.CancellationToken);

        Assert.NotEqual(Guid.Empty, executionContext.Id);
    }

    [Fact]
    public void ExecutionContext_FromNullTemplate_Succeeds()
    {
        // Act...
        var executionContext = new ExecutionContext(null);

        // Assert...
        Assert.NotNull(executionContext);
        Assert.NotNull(executionContext.CorrelationCallId);
        Assert.NotNull(executionContext.CorrelationId);

        Assert.Null(executionContext.Configuration);

        Assert.Equal(CultureInfo.InvariantCulture, executionContext.CultureInfo);
        Assert.Equal(CancellationToken.None, executionContext.CancellationToken);

        Assert.NotEqual(Guid.Empty, executionContext.Id);
    }

    [Fact]
    public void ExecutionContext_FromFullTemplate_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testExecutionContextTemplate = new ExecutionContextTemplate()
        {
            CancellationToken = new CancellationToken(faker.Random.Bool()),
            Configuration = new ConfigurationBuilder().AddInMemoryCollection(Enumerable.Range(0, faker.Random.Int(1, 10)).ToDictionary(i => i.ToString(), _ => faker.Random.Word())).Build(),
            CorrelationCallId = Guid.NewGuid().ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            CultureInfo = CultureInfo.GetCultureInfo(FakerProvider.GetFaker().Random.Int(1, 10)),
        };

        // Act...
        var executionContext = new ExecutionContext(testExecutionContextTemplate);

        // Assert...
        Assert.NotNull(executionContext);

        Assert.Equal(testExecutionContextTemplate.CorrelationCallId, executionContext.CorrelationCallId);
        Assert.Equal(testExecutionContextTemplate.CorrelationId, executionContext.CorrelationId);
        Assert.Equal(testExecutionContextTemplate.CultureInfo, executionContext.CultureInfo);
        Assert.Equal(testExecutionContextTemplate.CancellationToken, executionContext.CancellationToken);

        Assert.True(testExecutionContextTemplate.Configuration.IsSame(executionContext.Configuration));

        Assert.NotEqual(Guid.Empty, executionContext.Id);
    }

    [Fact]
    public void ExecutionContext_WithNullLogger_Succeeds()
    {
        // Act...
        var executionContext = new ExecutionContext<It.IsAnyType>(null);

        // Assert...
        Assert.Null(executionContext.Logger);
    }

    [Fact]
    public void ExecutionContext_WithLogger_Succeeds()
    {
        // Arrange...
        var mock = new Mock<ILogger<It.IsAnyType>>();
        var logger = mock.Object;

        // Act...
        var executionContext = new ExecutionContext<It.IsAnyType>(logger);

        // Assert...
        Assert.NotNull(executionContext.Logger);
        Assert.Same(logger, executionContext.Logger);
    }
}
