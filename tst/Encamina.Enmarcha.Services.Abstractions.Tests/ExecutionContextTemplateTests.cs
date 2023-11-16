using System.Globalization;

using Encamina.Enmarcha.Core.Extensions;
using Encamina.Enmarcha.Testing;

using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Services.Abstractions.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class ExecutionContextTemplateTests : FakerProviderFixturedBase
{
    public ExecutionContextTemplateTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void ExecutionContextTemplate_Empty_Succeeds()
    {
        // Act...
        var executionContextTemplate = new ExecutionContextTemplate();

        // Assert...
        Assert.NotNull(executionContextTemplate);

        Assert.Equal(CancellationToken.None, executionContextTemplate.CancellationToken);

        Assert.Null(executionContextTemplate.Configuration);
        Assert.Null(executionContextTemplate.CorrelationCallId);
        Assert.Null(executionContextTemplate.CorrelationId);
        Assert.Null(executionContextTemplate.CultureInfo);
    }

    [Fact]
    public void ExecutionContextTemplate_AllProperties_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var inMemorySettings = Enumerable.Range(0, faker.Random.Int(1, 10))
                                         .ToDictionary(i => i.ToString(), _ => faker.Random.Word());

        var testCancellationToken = new CancellationToken(faker.Random.Bool());
        var testConfiguration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();
        var testCorrelationCallId = Guid.NewGuid().ToString();
        var testCorrelationId = Guid.NewGuid().ToString();
        var testCultureInfo = CultureInfo.GetCultureInfo(FakerProvider.GetFaker().Random.Int(1, 10));

        // Act...
        var executionContextTemplate = new ExecutionContextTemplate()
        {
            CancellationToken = testCancellationToken,
            Configuration = testConfiguration,
            CorrelationCallId = testCorrelationCallId,
            CorrelationId = testCorrelationId,
            CultureInfo = testCultureInfo,
        };

        // Assert...
        Assert.NotNull(executionContextTemplate);

        Assert.Equal(testCancellationToken, executionContextTemplate.CancellationToken);
        Assert.Equal(testCorrelationCallId, executionContextTemplate.CorrelationCallId);
        Assert.Equal(testCorrelationId, executionContextTemplate.CorrelationId);
        Assert.Equal(testCultureInfo, executionContextTemplate.CultureInfo);

        Assert.True(executionContextTemplate.Configuration.IsSame(inMemorySettings));
    }
}
