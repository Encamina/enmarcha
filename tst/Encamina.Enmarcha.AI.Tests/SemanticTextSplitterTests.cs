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
        var semanticTextSplitter = new SemanticTextSplitter(optionsMonitor, ILengthFunctions.LengthByCharacterCount);

        embeddingsGeneratorMock
            .Setup(generator => generator(It.Is<IList<string>>(data => data.SequenceEqual(new[] { "This is a text that has 5 sentences.", "This one here is the second.", "This is the third.", "Here we have the fourth!", "And finally, the last one" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReadOnlyMemory<float>>(5)
            {
                new([99999999.0f, 98100f]), // This represents that the first sentence is very different
                new([2.0f, 1.0f]),
                new([25.0f, 8.0f]),
                new([3.0f, 1.0f]),
                new([7.0f, 1.0f]),
            });

        // Act...
        var splits = (await semanticTextSplitter.SplitAsync(text, embeddingsGeneratorMock.Object, CancellationToken.None)).ToList();

        // Assert...
        Assert.Equal(2, splits.Count);
        Assert.Equal("This is a text that has 5 sentences.", splits[0]);
        Assert.Equal("This one here is the second. This is the third. Here we have the fourth! And finally, the last one", splits[1]);
    }

    [Fact]
    public async Task SplitText_With_MaxChunkSizeAndRetryLimit_Succeeds()
    {
        // Arrange...
        const string text = "This is a text that has 4 sentences. This is the second sentence. Third. Here we have the last one!";
        var semanticTextSplitterOptions = GivenASemanticTextSplitterOptions(maxChunkSize: 50, chunkSplitRetryLimit: 1);
        var optionsMonitor = new TestOptionsMonitor<SemanticTextSplitterOptions>(semanticTextSplitterOptions);
        var semanticTextSplitter = new SemanticTextSplitter(optionsMonitor, ILengthFunctions.LengthByCharacterCount);

        embeddingsGeneratorMock
            .Setup(generator => generator(It.Is<IList<string>>(data => data.SequenceEqual(new[] { "This is a text that has 4 sentences.", "This is the second sentence.", "Third.", "Here we have the last one!" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReadOnlyMemory<float>>(3)
            {
                new([99999999.0f, 98100f]), // This represents that the first sentence is very different
                new([2.0f, 1.0f]),
                new([25.0f, 8.0f]),
            });

        embeddingsGeneratorMock
            .Setup(generator => generator(It.Is<IList<string>>(data => data.SequenceEqual(new[] { "This is the second sentence.", "Third.", "Here we have the last one!" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ReadOnlyMemory<float>>(3)
            {
                new([99999999.0f, 98100f]), // This represents that the first sentence is very different
                new([2.0f, 1.0f]),
                new([25.0f, 8.0f]),
            });

        // Act...
        var splits = (await semanticTextSplitter.SplitAsync(text, embeddingsGeneratorMock.Object, CancellationToken.None)).ToList();

        // Assert...
        Assert.Equal(3, splits.Count);
        Assert.Equal("This is a text that has 4 sentences.", splits[0]);
        Assert.Equal("This is the second sentence.", splits[1]);
        Assert.Equal("Third. Here we have the last one!", splits[2]);
    }

    [Fact]
    public async Task SplitText_With_SingleSentence_Returns_OriginalText()
    {
        // Arrange...
        const string singleSentence = "This is a single sentence.";
        var semanticTextSplitterOptions = GivenASemanticTextSplitterOptions();
        var optionsMonitor = new TestOptionsMonitor<SemanticTextSplitterOptions>(semanticTextSplitterOptions);
        var semanticTextSplitter = new SemanticTextSplitter(optionsMonitor, ILengthFunctions.LengthByCharacterCount);

        // Act...
        var splits = (await semanticTextSplitter.SplitAsync(singleSentence, embeddingsGeneratorMock.Object, CancellationToken.None)).ToList();

        // Assert...
        Assert.Single(splits);
        Assert.Equal(singleSentence, splits[0]);
    }

    private static SemanticTextSplitterOptions GivenASemanticTextSplitterOptions(int? maxChunkSize = null, int? chunkSplitRetryLimit = null)
    {
        return new SemanticTextSplitterOptions()
        {
            BufferSize = 0,
            BreakpointThresholdAmount = 95,
            BreakpointThresholdType = BreakpointThresholdType.Percentile,
            MaxChunkSize = maxChunkSize,
            ChunkSplitRetryLimit = chunkSplitRetryLimit,
        };
    }
}