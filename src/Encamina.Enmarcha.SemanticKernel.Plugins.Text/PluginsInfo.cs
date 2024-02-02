namespace Encamina.Enmarcha.SemanticKernel.Plugins.Text;

/// <summary>
/// Information about plugins in this assembly.
/// </summary>
public static class PluginsInfo
{
    /// <summary>
    /// Information about the «Text» plugin.
    /// </summary>
    public static class TextPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public static readonly string Name = nameof(TextPlugin);

        /// <summary>
        /// Information about the plugin's functions.
        /// </summary>
        public static class Functions
        {
            /// <summary>
            /// Information about the «KeyPhrases» function.
            /// </summary>
            public static class KeyPhrases
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(KeyPhrases);

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «input» parameter, which represents the text to analyze and extract keyphrases from.
                    /// </summary>
                    public static readonly string Input = nameof(Input).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «topKeyphrases» parameter, which represents the maximum or top number of keyphrases to extract.
                    /// </summary>
                    public static readonly string TopKeyphrases = nameof(TopKeyphrases).ToLowerInvariant();
                }
            }

            /// <summary>
            /// Information about the «KeyPhrasesLocaled» function.
            /// </summary>
            public static class KeyPhrasesLocaled
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(KeyPhrasesLocaled);

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «input» parameter, which represents the text to analyze and extract keyphrases from.
                    /// </summary>
                    public static readonly string Input = nameof(Input).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «locale» parameter, which represents the language in which the keyphrases will be generated.
                    /// </summary>
                    public static readonly string Locale = nameof(Locale).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «topKeyphrases» parameter, which represents the maximum or top number of keyphrases to extract.
                    /// </summary>
                    public static readonly string TopKeyphrases = nameof(TopKeyphrases).ToLowerInvariant();
                }
            }

            /// <summary>
            /// Information about the «Summarize» function.
            /// </summary>
            public static class Summarize
            {
                /// <summary>
                /// The name of the function.
                /// </summary>
                public static readonly string Name = nameof(Summarize);

                /// <summary>
                /// Information about the function's parameters.
                /// </summary>
                public static class Parameters
                {
                    /// <summary>
                    /// The name of the «input» parameter, which represents the text to summarize.
                    /// </summary>
                    public static readonly string Input = nameof(Input).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «minWordsCount» parameter, which represents the maximum number of words that the summary should have.
                    /// </summary>
                    public static readonly string MaxWordsCount = nameof(MaxWordsCount).ToLowerInvariant();

                    /// <summary>
                    /// The name of the «locale» parameter, which represents the language in which the summary will be generated.
                    /// </summary>
                    public static readonly string Locale = nameof(Locale).ToLowerInvariant();
                }
            }
        }
    }
}
