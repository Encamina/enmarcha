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
/// Extension methods on <see cref="IKernel"/> to import and configure plugins.
/// </summary>
public static class IKernelExtensions
{
    /// <summary>
    /// Imports the «Question Answering» plugin and its functions into the kernel.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IDictionary<string, ISKFunction> ImportQuestionAnsweringPlugin(this IKernel kernel, IServiceProvider serviceProvider, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNull(serviceProvider);
        Guard.IsNotNull(tokensLengthFunction);

        var semanticKernelOptions = serviceProvider.GetRequiredService<IOptionsMonitor<SemanticKernelOptions>>().CurrentValue;
        var modelName = semanticKernelOptions.CompletionsModelName ?? semanticKernelOptions.ChatModelName;

        var questionAnsweringPlugin = new QuestionAnsweringPlugin(kernel, modelName, tokensLengthFunction);

        var questionNativeFunc = kernel.ImportFunctions(questionAnsweringPlugin, PluginsInfo.QuestionAnsweringPlugin.Name);
        var questionSemanticFunc = kernel.ImportSemanticPluginsFromAssembly(Assembly.GetExecutingAssembly());

        return questionNativeFunc.Union(questionSemanticFunc).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Imports the «Question Answering» plugin and its functions into the kernel also adding the «Memory» plugin.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IDictionary<string, ISKFunction> ImportQuestionAnsweringPluginWithMemory(this IKernel kernel, IServiceProvider serviceProvider, Func<string, int> tokensLengthFunction)
    {
        var semanticTextMemory = serviceProvider.GetRequiredService<ISemanticTextMemory>();

        var questionAnsweringFunctions = kernel.ImportQuestionAnsweringPlugin(serviceProvider, tokensLengthFunction);

        var memoryFunc = kernel.ImportMemoryPlugin(semanticTextMemory, tokensLengthFunction);

        return memoryFunc.Union(questionAnsweringFunctions).ToDictionary(x => x.Key, x => x.Value);
    }
}
