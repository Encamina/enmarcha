using Microsoft.SemanticKernel.Connectors.AI.OpenAI.Tokenizers;

namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <inheritdoc/>
public interface ILengthFunctions : AI.Abstractions.ILengthFunctions
{
    /// <summary>
    /// Gets the number of tokens uisng <see cref="GPT3Tokenizer"/> on the specified text. If the text is <see langword="null"/> or empty (i.e., <see cref="string.Empty"/>), returns zero (<c>0</c>).
    /// </summary>
    public static Func<string, int> LengthByTokenCount => (text) => string.IsNullOrEmpty(text) ? 0 : GPT3Tokenizer.Encode(text).Count;
}