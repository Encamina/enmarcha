using Encamina.Enmarcha.SemanticKernel.Plugins.Memory.Plugins;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

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
    /// <param name="tokensLengthFunction">A function to count how many tokens are in a string or text.</param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static IDictionary<string, ISKFunction> ImportMemoryPlugin(this IKernel kernel, Func<string, int> tokensLengthFunction)
    {
        var memoryQueryPlugin = new MemoryQueryPlugin(kernel, tokensLengthFunction);

        return kernel.ImportSkill(memoryQueryPlugin, PluginsInfo.MemoryQueryPlugin.Name);
    }
}
