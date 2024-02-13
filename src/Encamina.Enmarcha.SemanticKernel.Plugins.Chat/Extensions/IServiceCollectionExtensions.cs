using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.Cosmos;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Options;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Extensions;

/// <summary>
/// Extension methods for setting up services in a <see cref="IServiceCollection"/>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    ///     <para>
    ///         Add and configures the <see cref="ChatHistoryProvider"/> type as singleton service instance of the <see cref="IChatHistoryProvider"/> service to the <see cref="IServiceCollection"/>.
    ///     </para>
    ///     <para>
    ///         Uses CosmosDB as the repository for chat history messages.
    ///     </para>
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="cosmosContainer">The name of the Cosmos DB container to store the chat history messages.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mixin» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <remarks>
    /// This extension method uses a «Service Location» pattern provided by the <see cref="IServiceProvider"/> to resolve the following dependencies:
    /// <list type="bullet">
    ///     <item>
    ///         <term>ChatHistoryProviderOptions</term>
    ///         <description>
    ///             A required dependency of type <see cref="ChatHistoryProviderOptions"/> used to retrieve the configuration options for this provider. This dependency should
    ///             be added using any of the <see cref="OptionsServiceCollectionExtensions.AddOptions"/> extension method.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ICosmosRepositoryFactory</term>
    ///         <description>
    ///             A required dependency of type <see cref="ICosmosRepositoryFactory"/> used to create a <see cref="ICosmosRepository{T}"/> (which
    ///             inherits from <see cref="IAsyncRepository{T}"/>) and manage chat history messages. Use the <c>AddCosmos</c> extension method to add this dependency.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCosmosChatHistoryProvider(this IServiceCollection services, string cosmosContainer, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNullOrWhiteSpace(cosmosContainer);
        Guard.IsNotNull(tokensLengthFunction);

        services.AddSingleton(sp =>
        {
            var chatMessagesHistoryRepository = sp.GetRequiredService<ICosmosRepositoryFactory>().Create<ChatMessageHistoryRecord>(cosmosContainer);
            var chatHistoryProviderOptions = sp.GetRequiredService<IOptionsMonitor<ChatHistoryProviderOptions>>();

            return new ChatHistoryProvider(tokensLengthFunction, chatMessagesHistoryRepository, chatHistoryProviderOptions);
        });

        services.AddSingleton<IChatHistoryProvider, ChatHistoryProvider>(sp => sp.GetRequiredService<ChatHistoryProvider>());

        return services;
    }
}