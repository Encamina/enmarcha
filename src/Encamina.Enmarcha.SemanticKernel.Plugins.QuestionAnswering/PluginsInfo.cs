using Encamina.Enmarcha.Core.Extensions;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

#pragma warning disable S3218 // Inner class members should not shadow outer class "static" or type members

/// <summary>
/// Information about plugins in this assembly.
/// </summary>
public static class PluginsInfo
{
    /// <summary>
    /// Information about the «Chat with History» plugin.
    /// </summary>
    public static class QuestionAnsweringPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public static readonly string Name = nameof(QuestionAnsweringPlugin);

        /// <summary>
        /// Information about the plugin's functions.
        /// </summary>
        public static class Functions
        {
            /// <summary>
            /// Information about the «Question Answering from Context» function.
            /// </summary>
            public static class QuestionAnsweringFromContext
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(QuestionAnsweringFromContext);

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «context» parameter, which represents contextual information that may contain the answer for question.
                    /// </summary>
                    public static readonly string Context = nameof(Context).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «input» parameter, which represents the question to answer with information from the context (<see cref="Context"/>).
                    /// </summary>
                    public static readonly string Input = nameof(Input).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «locale» parameter, which represents the language in which the response is generated. This parameter is optional. If not provided, the input (<see cref="Input"/>) language is used.
                    /// </summary>
                    public static readonly string Locale = nameof(Locale).ToLowerInvariant();
                }
            }

            /// <summary>
            /// Information about the «Question Answering from Memory» function.
            /// </summary>
            public static class QuestionAnsweringFromMemoryQuery
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(Plugins.QuestionAnsweringPlugin.QuestionAnsweringFromMemoryQueryAsync).RemoveAsyncSuffix();

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «question» parameter, used to look for an answer by searching a memory.
                    /// </summary>
                    public static readonly string Question = nameof(Question).ToLowerInvariant();

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

                    /// <summary>
                    /// The name of the «locale» parameter, which represents the language in which the response is generated. This parameter is optional. If not provided, the question (<see cref="Question"/>) language is used.
                    /// </summary>
                    public static readonly string Locale = nameof(Locale).ToLowerInvariant();
                }
            }
        }
    }
}

#pragma warning restore S3218 // Inner class members should not shadow outer class "static" or type members
