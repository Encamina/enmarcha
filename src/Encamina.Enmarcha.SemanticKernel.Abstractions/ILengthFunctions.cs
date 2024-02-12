using Microsoft.SemanticKernel.ChatCompletion;

using SharpToken;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <inheritdoc/>
public interface ILengthFunctions : AI.Abstractions.ILengthFunctions
{
    /// <summary>
    /// Gets the default <see cref="GptEncoding">encoding</see> for models like `GPT-3.5-Turbo` and `GPT-4` from OpenAI.
    /// </summary>
    public static readonly GptEncoding DefaultGptEncoding = GptEncoding.GetEncoding("cl100k_base");

    /// <summary>
    /// Dictionary to cache GptEncoding instances based on encoding names.
    /// </summary>
    private static readonly Dictionary<string, GptEncoding> EncodingCache = [];

    /// <summary>
    /// Gets the number of tokens using encodings for models like `GPT-3.5-Turbo` and `GPT-4` from OpenAI on the specified text.
    /// If the text is <see langword="null"/> or empty (i.e., <see cref="string.Empty"/>), returns zero (<c>0</c>).
    /// </summary>
    /// <seealso href="https://platform.openai.com/tokenizer"/>
    /// <seealso href="https://github.com/openai/openai-cookbook/blob/main/examples/How_to_count_tokens_with_tiktoken.ipynb"/>
    public static Func<string, int> LengthByTokenCount => (text) => string.IsNullOrEmpty(text) ? 0 : DefaultGptEncoding.Encode(text).Count;

    /// <summary>
    /// Gets the number of tokens using a given encoding on the specified text.
    /// If the text is <see langword="null"/> or empty (i.e., <see cref="string.Empty"/>), returns zero (<c>0</c>).
    /// </summary>
    /// <seealso href="https://github.com/openai/openai-cookbook/blob/main/examples/How_to_count_tokens_with_tiktoken.ipynb"/>
    public static Func<string, string, int> LengthByTokenCountUsingEncoding => (encoding, text) => string.IsNullOrEmpty(text) ? 0 : GetCachedEncoding(encoding).Encode(text).Count;

    /// <summary>
    /// Calculates the length of a chat message with the specified content and author role, using a provided length function.
    /// </summary>
    /// <param name="content">The content of the chat message.</param>
    /// <param name="authorRole">The <see cref="AuthorRole"/> of the message.</param>
    /// <param name="lengthFunction">A function to calculate the length of a string.</param>
    /// <returns>The total length for the chat message.</returns>
    public static int LengthChatMessage(string content, AuthorRole authorRole, Func<string, int> lengthFunction)
        => InnerLengthChatMessage(content, null, authorRole, (s, _) => lengthFunction(s));

    /// <summary>
    /// Calculates the length of a chat message with the specified content, encoding and author role, using a provided length function with encoding.
    /// </summary>
    /// <param name="content">The content of the chat message.</param>
    /// <param name="encoding">The name of the GptEncoding.</param>
    /// <param name="authorRole">The <see cref="AuthorRole"/> of the message.</param>
    /// <param name="lengthFunctionWithEncoding">A function to calculate the length of a string with encoding.</param>
    /// <returns>The total length for the chat message.</returns>
    public static int LengthChatMessageWithEncoding(string content, string encoding, AuthorRole authorRole, Func<string, string, int> lengthFunctionWithEncoding)
        => InnerLengthChatMessage(content, encoding, authorRole, lengthFunctionWithEncoding);

    /// <summary>
    /// Internal method to calculate the length of a chat message with the specified content, encoding and author role, using a provided length function with encoding.
    /// </summary>
    /// <param name="content">The content of the chat message.</param>
    /// <param name="encoding">The name of the GptEncoding.</param>
    /// <param name="authorRole">The <see cref="AuthorRole"/> of the message.</param>
    /// <param name="lengthFunction">A function to calculate the length of a string.</param>
    /// <returns>The total length for the chat message.</returns>
    private static int InnerLengthChatMessage(string content, string encoding, AuthorRole authorRole, Func<string, string, int> lengthFunction)
    {
        var tokenCount = authorRole == AuthorRole.System ? lengthFunction(encoding, "\n") : 0;
        return tokenCount + lengthFunction(encoding, $"role:{authorRole.Label}") + lengthFunction(encoding, $"content:{content}");
    }

    /// <summary>
    /// Gets the GptEncoding instance based on the specified encoding name, caching it for future use.
    /// </summary>
    /// <param name="encoding">The name of the GptEncoding.</param>
    /// <returns>The GptEncoding instance.</returns>
    private static GptEncoding GetCachedEncoding(string encoding)
    {
        if (EncodingCache.TryGetValue(encoding, out var gptEncoding))
        {
            return gptEncoding;
        }

        gptEncoding = GptEncoding.GetEncoding(encoding);
        EncodingCache[encoding] = gptEncoding;
        return gptEncoding;
    }
}
