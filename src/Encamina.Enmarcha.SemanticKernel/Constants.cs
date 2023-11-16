namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// Public constants for common Semantic Kernel concepts and features.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The name of the semantic functions configuration file.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/prompt-engineering/configure-prompts"/>
    public static readonly string ConfigFile = @"config.json";

    /// <summary>
    /// The name of the prompt template file.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/semantic-kernel/prompt-engineering/prompt-template-syntax"/>
    public static readonly string PromptFile = @"skprompt.txt";

    /// <summary>
    /// The name of a metadta value that contains the total number of chunks in a memory.
    /// </summary>
    public static readonly string MetadataTotalChunksCount = @"MetadataTotalChunksCount";
}
