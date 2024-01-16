using System.Reflection;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Extensions;
using Encamina.Enmarcha.Testing;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Tests;

[Collection(MagicStrings.FixturesCollection)]
public class KernelExtensionsTests : FakerProviderFixturedBase
{
    public KernelExtensionsTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void ImportPromptFunctionsFromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();

        // Act...
        var kernelPlugin = kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());

        // Assert..
        var plugin = Assert.Single(kernelPlugin);
        Assert.Equal("PromptPlugins", plugin.Name);

        var function = Assert.Single(plugin.ToList());
        Assert.Equal("DummyEmbedded", function.Name);
    }

    [Fact]
    public async Task GetKernelFunctionPromptAsync_FromPluginDirectory_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestUtilities", "PromptPlugins"));
        var function = kernel.Plugins.GetFunction("PromptPlugins", "Dummy");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var prompt = await kernel.GetKernelFunctionPromptAsync("TestUtilities", function, arguments);

        // Assert..
        Assert.Equal("This is a Prompt function for testing purposes dummy foo value bar value", prompt);
    }

    [Fact]
    public async Task GetKernelFunctionPromptAsync_FromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());
        var function = kernel.Plugins.GetFunction("PromptPlugins", "DummyEmbedded");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var prompt = await kernel.GetKernelFunctionPromptAsync(Assembly.GetExecutingAssembly(), function, arguments);

        // Assert..
        Assert.Equal("This is a Prompt function for testing purposes dummy foo value bar value", prompt);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokensAsync_FromPluginDirectory_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestUtilities", "PromptPlugins"));
        var function = kernel.Plugins.GetFunction("PromptPlugins", "Dummy");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };  
          
        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensAsync("TestUtilities", function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + 500; // prompt with arguments + config json max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokensAsync_FromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());
        var function = kernel.Plugins.GetFunction("PromptPlugins", "DummyEmbedded");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensAsync(Assembly.GetExecutingAssembly(), function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + 500; // prompt with arguments + config json max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    private Kernel GivenAKernel()
    {
        return Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(FakerProvider.GetFaker().Random.Word(), 
                FakerProvider.GetFaker().Internet.Url(),
                FakerProvider.GetFaker().Random.Guid().ToString()) 
            .Build();
    }
}