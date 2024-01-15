using System.Reflection;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Extensions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Memory;
using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.Plugins;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

/// <summary>
/// Extension methods on <see cref="Kernel"/> to import and configure plugins.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Imports the «Question Answering» plugin and its functions into the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IEnumerable<KernelPlugin> ImportQuestionAnsweringPlugin(this Kernel kernel, IServiceProvider serviceProvider, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNull(serviceProvider);
        Guard.IsNotNull(tokensLengthFunction);

        var semanticKernelOptions = serviceProvider.GetRequiredService<IOptionsMonitor<SemanticKernelOptions>>().CurrentValue;

        kernel.ImportPluginFromObject(new QuestionAnsweringPlugin(kernel, semanticKernelOptions.CompletionsModelName ?? semanticKernelOptions.ChatModelName, tokensLengthFunction), PluginsInfo.QuestionAnsweringPlugin.Name);
        kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());

        return kernel.Plugins;
    }

    /// <summary>
    /// Imports the «Question Answering» plugin and its functions into the kernel also adding the «Memory» plugin.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IEnumerable<KernelPlugin> ImportQuestionAnsweringPluginWithMemory(this Kernel kernel, IServiceProvider serviceProvider, Func<string, int> tokensLengthFunction)
    {
        var semanticTextMemory = serviceProvider.GetRequiredService<ISemanticTextMemory>();

        kernel.ImportQuestionAnsweringPlugin(serviceProvider, tokensLengthFunction);
        kernel.ImportMemoryPlugin(semanticTextMemory, tokensLengthFunction);

        return kernel.Plugins;
    }
}
