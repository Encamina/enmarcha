using System.Reflection;

using Encamina.Enmarcha.SemanticKernel.Extensions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Text;

/// <summary>
/// Extension methods on <see cref="IKernel"/> to import and configure plugins.
/// </summary>
public static class IKernelExtensions
{
    /// <summary>
    /// Imports the «Text» plugin and its functions into the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> instance to add this plugin.</param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static IDictionary<string, ISKFunction> ImportTextPlugin(this IKernel kernel)
    {
        return kernel.ImportSemanticPluginsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

