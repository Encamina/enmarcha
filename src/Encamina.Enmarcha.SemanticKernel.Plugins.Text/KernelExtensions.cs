using System.Reflection;

using Encamina.Enmarcha.SemanticKernel.Extensions;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Text;

/// <summary>
/// Extension methods on <see cref="Kernel"/> to import and configure plugins.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Imports the «Text» plugin and its functions into the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> instance to add this plugin.</param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static IEnumerable<KernelPlugin> ImportTextPlugin(this Kernel kernel)
    {
        return kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

