using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Extensions;

/// <summary>
/// Extension methods on <see cref="Kernel"/> to import and configure plugins.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Imports the «Chat with History» plugin into the <see cref="Kernel"/>.
    /// </summary>
    /// <remarks>
    /// This extension method uses a «Service Location» pattern provided by the <see cref="IServiceProvider"/> to resolve the following dependencies:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ChatWithHistoryPluginOptions</term>
    ///         <description>
    ///             A required dependency of type <see cref="ChatWithHistoryPluginOptions"/> used to retrieve the configuration options for this plugin. This dependency should
    ///             be added using any of the <see cref="OptionsServiceCollectionExtensions.AddOptions"/> extension method.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ChatHistoryProvider</term>
    ///         <description>
    ///             A required dependency of type <see cref="IChatHistoryProvider"/> used to create and manage chat history messages.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="kernel">The <see cref="Kernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="openAIOptions">Configuration options for OpenAI services.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mixin» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static KernelPlugin ImportChatWithHistoryPlugin(this Kernel kernel, IServiceProvider serviceProvider, OpenAIOptions openAIOptions, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNull(serviceProvider);
        Guard.IsNotNull(openAIOptions);
        Guard.IsNotNull(tokensLengthFunction);

        var chatWithHistoryPluginOptions = serviceProvider.GetRequiredService<IOptionsMonitor<ChatWithHistoryPluginOptions>>();
        var chatHistoryProvider = serviceProvider.GetRequiredService<IChatHistoryProvider>();

        var chatWithHistoryPlugin = new ChatWithHistoryPlugin(kernel, openAIOptions.ChatModelName, tokensLengthFunction, chatHistoryProvider, chatWithHistoryPluginOptions);

        return kernel.ImportPluginFromObject(chatWithHistoryPlugin, PluginsInfo.ChatWithHistoryPlugin.Name);
    }
}
