using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat;

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members

/// <summary>
/// Information about plugins in this assembly.
/// </summary>
public static class PluginsInfo
{
    /// <summary>
    /// Information about the «Chat with History» plugin.
    /// </summary>
    public static class ChatWithHistoryPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public static readonly string Name = nameof(Plugins.ChatWithHistoryPlugin);

        /// <summary>
        /// Information about the plugin's functions.
        /// </summary>
        public static class Functions
        {
            /// <summary>
            /// Information about the «Chat» function.
            /// </summary>
            public static class Chat
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(Plugins.ChatWithHistoryPlugin.ChatAsync).RemoveAsyncSuffix();

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «ask» parameter, which represents what the user says or asks when chatting.
                    /// </summary>
                    public static readonly string Ask = nameof(Ask).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «userId» parameter, which represents a unique identifier for the user when chatting.
                    /// </summary>
                    public static readonly string UserId = nameof(UserId).ToLowerInvariant();

                    /// <summary>
                    ///  The name of the «userName» parameter, which represents the name of the user when chatting.
                    /// </summary>
                    public static readonly string UserName = nameof(UserName).ToLowerInvariant();

                    /// <summary>
                    ///  The name of the «locale» parameter, which represents the preferred language of the user while chatting.
                    /// </summary>
                    public static readonly string Locale = nameof(Locale).ToLowerInvariant();
                }
            }
        }
    }
}

#pragma warning restore S3218 // Inner class members should not shadow outer class "static" or type members
