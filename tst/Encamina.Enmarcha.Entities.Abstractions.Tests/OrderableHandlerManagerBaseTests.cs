using Encamina.Enmarcha.Testing;

namespace Encamina.Enmarcha.Entities.Abstractions.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class OrderableHandlerManagerBaseTests : FakerProviderFixturedBase
{
    public OrderableHandlerManagerBaseTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void Create_Null_OrderableHandlerManager_Succeeds()
    {
        // Arrange...
        OrderableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(null);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.Null(handlers);
    }

    [Fact]
    public void Create_Empty_OrderableHandlerManager_Succeeds()
    {
        // Arrange...
        OrderableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(Enumerable.Empty<TestHandler>());

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.Empty(handlers);
    }

    [Fact]
    public void Create_OrderableHandlerManager_Succeeds()
    {
        // Arrange...
        var testHandlers = Enumerable.Range(1, FakerProvider.GetFaker().Random.Int(2, 100))
                                     .Distinct()
                                     .Select(n => new TestHandler(n))
                                     .ToList(); // Force enumeration to have the same order values during this test.

        var isSorted = (IEnumerable<TestHandler> hdlrs) =>
        {
            for (var i = 1; i < hdlrs.Count(); i++)
            {
                if (hdlrs.ElementAt(i - 1).Order > hdlrs.ElementAt(i).Order)
                {
                    return false;
                }
            }

            return true;
        };

        OrderableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(testHandlers);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.NotEmpty(handlers);
        Assert.True(handlers.Count() == testHandlers.Count && isSorted(handlers)); // Two dictionaries are equivalent if they are the same size and there are no elements which are in the first and not in the second.
    }

    private sealed class TestHandler : IOrderable
    {
        public TestHandler(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    private sealed class TestHandlerManager : OrderableHandlerManagerBase<TestHandler>
    {
        public TestHandlerManager(IEnumerable<TestHandler> handlers) : base(handlers)
        {
        }
    }
}
