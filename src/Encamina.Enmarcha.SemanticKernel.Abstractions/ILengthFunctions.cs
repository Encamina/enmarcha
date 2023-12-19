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
    private static readonly Dictionary<string, GptEncoding> EncodingCache = new Dictionary<string, GptEncoding>();

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