using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.TextSplitters;
using Encamina.Enmarcha.Testing;

namespace Encamina.Enmarcha.AI.Tests;

public sealed class RecursiveCharacterTextSplitterTests
{
    [Fact]
    public void SplitText_WithDefaultTextSplitterOptions_Succeeds()
    {
        // Arrange...
        var defaultTextSplitterOptions = new TextSplitterOptions()
        {
            Separators = new List<string>() { "\n\n", "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 100,
        };
        var optionsMonitor = new TestOptionsMonitor<TextSplitterOptions>(defaultTextSplitterOptions);
        var recursiveCharacterTextSplitter = new RecursiveCharacterTextSplitter(optionsMonitor);
        var lengthFunction = ILengthFunctions.LengthByCharacterCount;
        var text = GivenAText();

        // Act...
        var splits = recursiveCharacterTextSplitter.Split(text, lengthFunction).ToList();

        // Assert...
        Assert.Equal(10, splits.Count);
        Assert.Equal("Lorem ipsum dolor sit amet\n\nConsectetur adipiscing elit", splits[0]);
        Assert.Equal("tortor vitae purus faucibus ornare suspendisse.", splits[9]);
    }

    [Fact]
    public void SplitText_WithParameterTextSplitterOptions_Succeeds()
    {
        // Arrange...
        var dummyTextSplitterOptions = GivenATextSplitterOptions();
        var optionsMonitor = new TestOptionsMonitor<TextSplitterOptions>(dummyTextSplitterOptions);
        var recursiveCharacterTextSplitter = new RecursiveCharacterTextSplitter(optionsMonitor);
        var textSplitterOptions = new TextSplitterOptions()
        {
            Separators = new List<string>() { "\n\n", "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 100,
        };
        var lengthFunction = ILengthFunctions.LengthByCharacterCount;
        var text = GivenAText();

        // Act...
        var splits = recursiveCharacterTextSplitter.Split(text, lengthFunction, textSplitterOptions).ToList();

        // Assert...
        Assert.Equal(10, splits.Count);
        Assert.Equal("Lorem ipsum dolor sit amet\n\nConsectetur adipiscing elit", splits[0]);
        Assert.Equal("tortor vitae purus faucibus ornare suspendisse.", splits[9]);
    }

    [Fact]
    public void SplitText_WithParameterTextSplitterOptions_NotAlter_DefaultTextSplitterOptions()
    {
        // Arrange...
        var dummyTextSplitterOptions = GivenATextSplitterOptions();
        var textSplitterOptions = new TextSplitterOptions()
        {
            Separators = new List<string>() { "\n\n", "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 100,
        };
        var optionsMonitor = new TestOptionsMonitor<TextSplitterOptions>(textSplitterOptions);
        var recursiveCharacterTextSplitter = new RecursiveCharacterTextSplitter(optionsMonitor);
        var lengthFunction = ILengthFunctions.LengthByCharacterCount;
        var text = GivenAText();

        // Act...
        var splitsWithParameterOptions = recursiveCharacterTextSplitter.Split(text, lengthFunction, dummyTextSplitterOptions).ToList();
        var splitsWithDefaultOptions = recursiveCharacterTextSplitter.Split(text, lengthFunction).ToList();

        // Assert...
        Assert.Equal(81, splitsWithParameterOptions.Count);
        Assert.Equal(10, splitsWithDefaultOptions.Count);
        Assert.Equal("Lorem ipsum dolor sit amet\n\nConsectetur adipiscing elit", splitsWithDefaultOptions[0]);
        Assert.Equal("tortor vitae purus faucibus ornare suspendisse.", splitsWithDefaultOptions[9]);
    }

    [Fact]
    public void SplitText_WithEmptyString_ReturnsNoSplits()
    {
        // Arrange
        var defaultTextSplitterOptions = new TextSplitterOptions
        {
            Separators = new List<string> { "\n\n", "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 100,
        };
        var optionsMonitor = new TestOptionsMonitor<TextSplitterOptions>(defaultTextSplitterOptions);
        var recursiveCharacterTextSplitter = new RecursiveCharacterTextSplitter(optionsMonitor);
        var lengthFunction = ILengthFunctions.LengthByCharacterCount;
        var text = string.Empty;

        // Act
        var splits = recursiveCharacterTextSplitter.Split(text, lengthFunction).ToList();

        // Assert
        Assert.Empty(splits);
    }

    [Fact]
    public void SplitText_WithSingleWord_ReturnsSingleSplit()
    {
        // Arrange
        var defaultTextSplitterOptions = new TextSplitterOptions
        {
            Separators = new List<string> { "\n\n", "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 100,
        };
        var optionsMonitor = new TestOptionsMonitor<TextSplitterOptions>(defaultTextSplitterOptions);
        var recursiveCharacterTextSplitter = new RecursiveCharacterTextSplitter(optionsMonitor);
        var lengthFunction = ILengthFunctions.LengthByCharacterCount;
        var text = "Word";

        // Act
        var splits = recursiveCharacterTextSplitter.Split(text, lengthFunction).ToList();

        // Assert
        Assert.Single(splits);
        Assert.Equal("Word", splits[0]);
    }

    private static string GivenAText()
    {
        return """
               Lorem ipsum dolor sit amet

               Consectetur adipiscing elit

               sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Pellentesque sit amet porttitor eget dolor morbi non. Cursus metus aliquam eleifend mi in. Tellus rutrum tellus pellentesque eu tincidunt tortor aliquam. Arcu felis bibendum ut tristique et egestas. Nunc consequat interdum varius sit.
               Dignissim convallis aenean et tortor at risus viverra adipiscing. Ut faucibus pulvinar elementum integer enim neque. Tincidunt vitae semper quis lectus nulla. Enim blandit volutpat maecenas volutpat blandit aliquam etiam erat velit. Tempus egestas sed sed risus pretium. Duis at consectetur lorem donec massa sapien. Magna etiam tempor orci eu. Suspendisse potenti nullam ac tortor vitae purus faucibus ornare suspendisse.
               """;
    }

    private static TextSplitterOptions GivenATextSplitterOptions()
    {
        return new TextSplitterOptions()
        {
            Separators = new List<string>() { "\n", " ", string.Empty },
            ChunkOverlap = 0,
            ChunkSize = 20,
        };
    }
}