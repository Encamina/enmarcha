namespace Encamina.Enmarcha.SemanticKernel.Abstractions;

/// <summary>
/// Represents the content of a memory.
/// </summary>
public sealed class MemoryContent
{
    /// <summary>
    /// Gets the metadata of the memory.
    /// </summary>
    public IDictionary<string, string> Metadata { get; init; }

    /// <summary>
    /// Gets the chunks of the memory.
    /// </summary>
    public IEnumerable<string> Chunks { get; init; }
}
