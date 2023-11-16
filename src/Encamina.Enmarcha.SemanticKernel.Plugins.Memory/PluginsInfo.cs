using System.Reflection;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.SemanticKernel.SkillDefinition;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Memory;

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members

/// <summary>
/// Information about plguins in this assembly.
/// </summary>
public static class PluginsInfo
{
    /// <summary>
    /// Information about the «Memory Query» plugin.
    /// </summary>
    public static class MemoryQueryPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public static readonly string Name = nameof(Plugins.MemoryQueryPlugin);

        /// <summary>
        /// Information about the plugin's functions.
        /// </summary>
        public static class Functions
        {
            /// <summary>
            /// Informnation about the «Query Memory» function.
            /// </summary>
            public static class QueryMemory
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(Plugins.MemoryQueryPlugin.QueryMemoryAsync).RemoveAsyncSuffix();

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «query» parameter, used to to search the memory.
                    /// </summary>
                    public static readonly string Query = nameof(Query).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «collectionStr» parameter, which represents a list of memory's collection names, usually comma-separated.
                    /// </summary>
                    /// <remarks>
                    /// The separation character is set by the «collectionSeparator» parameter (<see cref="CollectionSeparator"/>).
                    /// </remarks>
                    public static readonly string CollectionsStr = nameof(CollectionsStr).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «collectionSeparator» parameter, which represents the character (usually a comma) that separates each collection's name
                    /// from the given list of collections (<see cref="CollectionsStr"/>).
                    /// </summary>
                    public static readonly string CollectionSeparator = nameof(CollectionSeparator).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «responseTokenLimit» parameter, which represents the available maximum number of tokens for the answer.
                    /// </summary>
                    public static readonly string ResponseTokenLimit = nameof(ResponseTokenLimit).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «minRelevance» parameter, which represents the minimum expected relevance for the search results when searching the memory.
                    /// </summary>
                    public static readonly string MinRelevance = nameof(MinRelevance).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «resultsLimit» parameter, which represents the maximum number of results per queried collection.
                    /// </summary>
                    public static readonly string ResultsLimit = nameof(ResultsLimit).ToLowerInvariant();
                }
            }
        }
    }
}

#pragma warning restore S3218 // Inner class members should not shadow outer class "static" or type members
