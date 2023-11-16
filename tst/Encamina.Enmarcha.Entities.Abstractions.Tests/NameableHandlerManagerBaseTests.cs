using Encamina.Enmarcha.Testing;

namespace Encamina.Enmarcha.Entities.Abstractions.Tests;

[Collection(MagicStrings.FixturesCollection)]
public sealed class NameableHandlerManagerBaseTests : FakerProviderFixturedBase
{
    public NameableHandlerManagerBaseTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void Create_Null_NameableHandlerManager_Succeeds()
    {
        // Arrange...
        NameableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(null);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.Null(handlers);
    }

    [Fact]
    public void Create_Empty_NameableHandlerManager_Succeeds()
    {
        // Arrange...
        NameableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(Enumerable.Empty<TestHandler>());

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.Empty(handlers);
    }

    [Fact]
    public void Create_NameableHandlerManager_Succeeds()
    {
        // Arrange...
        var faker = FakerProvider.GetFaker();

        var testHandlers = Enumerable.Range(1, faker.Random.Int(2, 100))
                                     .Select(_ => faker.Random.Word())
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .Select(n => new TestHandler(n))
                                     .ToList(); // Force enumeration to have the same random words during this test.

        var testHandlersDictionary = testHandlers.ToDictionary(i => i.Name);

        NameableHandlerManagerBase<TestHandler> handlerManager = new TestHandlerManager(testHandlers);

        // Act...
        var handlers = handlerManager.Handlers;

        // Assert...
        Assert.NotNull(handlers);
        Assert.NotEmpty(handlers);
        Assert.True(handlers.Count == testHandlersDictionary.Count && !handlers.Except(testHandlersDictionary).Any()); // Two dictionaries are equivalent if they are the same size and there are no elements which are in the first and not in the second.
    }

    private sealed class TestHandler : INameable
    {
        public TestHandler(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    private sealed class TestHandlerManager : NameableHandlerManagerBase<TestHandler>
    {
        public TestHandlerManager(IEnumerable<TestHandler> handlers) : base(handlers)
        {
        }
    }
}
