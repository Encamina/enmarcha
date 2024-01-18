using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Tests.TestUtilities.PluginTest;

public class DummyNative
{
    [KernelFunction]
    public string SayHello()
    {
        return "Hello world";
    }
}
