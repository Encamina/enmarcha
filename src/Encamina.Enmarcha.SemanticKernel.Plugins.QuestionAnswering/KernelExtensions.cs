using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Memory;
using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.Plugins;

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
    /// <param name="openAIOptions">Options to connect and use an OpenAI service.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IEnumerable<KernelPlugin> ImportQuestionAnsweringPlugin(this Kernel kernel, OpenAIOptionsBase openAIOptions, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNull(openAIOptions);
        Guard.IsNotNull(tokensLengthFunction);

        kernel.ImportPluginFromObject(new QuestionAnsweringPlugin(kernel, openAIOptions.CompletionsModelName ?? openAIOptions.ChatModelName, tokensLengthFunction), PluginsInfo.QuestionAnsweringPlugin.Name);

        return kernel.Plugins;
    }

    /// <summary>
    /// Imports the «Question Answering» plugin and its functions into the kernel also adding the «Memory» plugin.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> instance to add this plugin.</param>
    /// <param name="openAIOptions">Options to connect and use an OpenAI service.</param>
    /// <param name="semanticTextMemory">A valid instance of <see cref="ISemanticTextMemory"/> to use as memory for this plugin.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mix-in» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    /// <seealso href="https://en.wikipedia.org/wiki/Mixin"/>
    public static IEnumerable<KernelPlugin> ImportQuestionAnsweringPluginWithMemory(this Kernel kernel, OpenAIOptionsBase openAIOptions, ISemanticTextMemory semanticTextMemory, Func<string, int> tokensLengthFunction)
    {
        kernel.ImportQuestionAnsweringPlugin(openAIOptions, tokensLengthFunction);
        kernel.ImportMemoryPlugin(semanticTextMemory, tokensLengthFunction);

        return kernel.Plugins;
    }
}
