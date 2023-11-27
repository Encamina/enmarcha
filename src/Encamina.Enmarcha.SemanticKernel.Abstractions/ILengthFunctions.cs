using SharpToken;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <inheritdoc/>
public interface ILengthFunctions : AI.Abstractions.ILengthFunctions
{
    /// <summary>
    /// Gets the number of tokens using encodings for models like `GPT-3.5-Turbo` and `GPT-4` from OpenAI on the specified text.
    /// If the text is <see langword="null"/> or empty (i.e., <see cref="string.Empty"/>), returns zero (<c>0</c>).
    /// </summary>
    /// <seealso href="https://platform.openai.com/tokenizer"/>
    public static Func<string, int> LengthByTokenCount => (text) => string.IsNullOrEmpty(text) ? 0 : GptEncoding.GetEncoding("cl100k_base").Encode(text).Count;

    /// <summary>
    /// Gets the number of tokens using a given encoding on the specified text.
    /// If the text is <see langword="null"/> or empty (i.e., <see cref="string.Empty"/>), returns zero (<c>0</c>).
    /// </summary>
    public static Func<string, string, int> LengthByTokenCountUsingEncoding => (encoding, text) => string.IsNullOrEmpty(text) ? 0 : GptEncoding.GetEncoding(encoding).Encode(text).Count;
}