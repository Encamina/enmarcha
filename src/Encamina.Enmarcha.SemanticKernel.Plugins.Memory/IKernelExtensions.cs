using Encamina.Enmarcha.SemanticKernel.Plugins.Memory.Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Memory;

/// <summary>
/// Extension methods on <see cref="IKernel"/> to import and configure plugins.
/// </summary>
public static class IKernelExtensions
{
    /// <summary>
    /// Imports the «Memory» plugin and its functions into the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> instance to add this plugin.</param>
    /// <param name="semanticTextMemory">A valid instance of a semantic memory to recall memories associated with text.</param>
    /// <param name="tokensLengthFunction">A function to count how many tokens are in a string or text.</param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static IDictionary<string, ISKFunction> ImportMemoryPlugin(this IKernel kernel, ISemanticTextMemory semanticTextMemory, Func<string, int> tokensLengthFunction)
    {
        var memoryQueryPlugin = new MemoryQueryPlugin(semanticTextMemory, tokensLengthFunction);

        return kernel.ImportFunctions(memoryQueryPlugin, PluginsInfo.MemoryQueryPlugin.Name);
    }
}
