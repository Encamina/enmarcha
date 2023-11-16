using Encamina.Enmarcha.Testing;

namespace Encamina.Enmarcha.Entities.Abstractions.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class HandlerManagerBaseTests : FakerProviderFixturedBase
{
    public HandlerManagerBaseTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void Create_Null_HandlerManager_Succeeds()
    {
        // Arrange...
        HandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(null);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.Null(handlers);
    }

    [Fact]
    public void Create_Empty_HandlerManager_Succeeds()
    {
        // Arrange...
        HandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(Enumerable.Empty<TestHandler>());

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.Empty(handlers);
    }

    [Fact]
    public void Create_HandlerManager_Succeeds()
    {
        // Arrange...
        var testHandlers = Enumerable.Range(1, FakerProvider.GetFaker().Random.Int(2, 100)).Select(_ => new TestHandler());

        HandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(testHandlers);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.NotEmpty(handlers);
        Assert.True(handlers.Count() == testHandlers.Count());
    }

    private sealed class TestHandler
    {
    }

    private sealed class TestHandlerManager : HandlerManagerBase<TestHandler>
    {
        public TestHandlerManager(IEnumerable<TestHandler> handlers) : base(handlers)
        {
        }
    }
}