using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.TextSplitters;
using Encamina.Enmarcha.Testing;

using Moq;

namespace Encamina.Enmarcha.AI.Tests;

public sealed class SemanticTextSplitterTests
{
    private readonly Mock<Func<IList<string>, CancellationToken, Task<IList<ReadOnlyMemory<float>>>>> embeddingsGeneratorMock = new(MockBehavior.Strict);

    [Fact]
    public async Task SplitText_Succeeds()
    {
        // Arrange...
        const string text = "This is a text that has 5 sentences. This one here is the second. This is the third. Here we have the fourth! And finally, the last one";
        var semanticTextSplitterOptions = GivenASemanticTextSplitterOptions();
        var optionsMonitor = new TestOptionsMonitor<SemanticTextSplitterOptions>(semanticTextSplitterOptions);
        var semanticTextSplitter = new SemanticTextSplitter(optionsMonitor);

        embeddingsGeneratorMock
            .Setup(generator => generator(It.Is<IList<string>>(data => data.SequenceEqual(new[] { "This is a text that has 5 sentences.", "This one here is the second.", "This is the third.", "Here we have the fourth!", "And finally, the last one" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReadOnlyMemory<float>>(5)
            {
                new([99999999.0f, 98100f]), // This represents that the first sentence is very different
                new([2.0f, 1.0f]),
                new([25.0f, 8.0f]),
                new([3.0f, 1.0f]),
                new([7.0f, 1.0f]),
                new([10.0f, 4.0f]),
            });

        // Act...
        var splits = (await semanticTextSplitter.SplitAsync(text, embeddingsGeneratorMock.Object, CancellationToken.None)).ToList();

        // Assert...
        Assert.Equal(2, splits.Count);
        Assert.Equal("This is a text that has 5 sentences.", splits[0]);
        Assert.Equal("This one here is the second. This is the third. Here we have the fourth! And finally, the last one", splits[1]);
    }

    [Fact]
    public async Task SplitText_With_SingleSentence_Returns_OriginalText()
    {
        // Arrange...
        const string singleSentence = "This is a single sentence.";
        var semanticTextSplitterOptions = GivenASemanticTextSplitterOptions();
        var optionsMonitor = new TestOptionsMonitor<SemanticTextSplitterOptions>(semanticTextSplitterOptions);
        var semanticTextSplitter = new SemanticTextSplitter(optionsMonitor);

        // Act...
        var splits = (await semanticTextSplitter.SplitAsync(singleSentence, embeddingsGeneratorMock.Object, CancellationToken.None)).ToList();

        // Assert...
        Assert.Single(splits);
        Assert.Equal(singleSentence, splits[0]);
    }

    private static SemanticTextSplitterOptions GivenASemanticTextSplitterOptions()
    {
        return new SemanticTextSplitterOptions()
        {
            BufferSize = 0,
            BreakpointThresholdAmount = 95,
            BreakpointThresholdType = BreakpointThresholdType.Percentile,
        };
    }
}