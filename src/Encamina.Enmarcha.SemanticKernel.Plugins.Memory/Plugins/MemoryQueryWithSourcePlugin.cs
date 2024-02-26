using System.Text.Json;

using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Memory.Plugins;

/// <summary>
/// Plugin to query a memory and add the source of the information to the result.
/// </summary>
public class MemoryQueryWithSourcePlugin : MemoryQueryPlugin
{
    private readonly string metadataSourceKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryQueryWithSourcePlugin"/> class.
    /// </summary>
    /// <param name="semanticTextMemory">A valid instance of a semantic memory to recall memories associated with text.</param>
    /// <param name="tokenLengthFunction">Function to calculate the length of a string in tokens.</param>
    /// <param name="metadataSourceKey">The key to use to retrieve the source of the information from the metadata of each chunk.</param>
    public MemoryQueryWithSourcePlugin(ISemanticTextMemory semanticTextMemory, Func<string, int> tokenLengthFunction, string metadataSourceKey) : base(semanticTextMemory, tokenLengthFunction)
    {
        this.metadataSourceKey = metadataSourceKey;
    }

    /// <inheritdoc/>
    protected override MemoryQueryResult ParseMemoryQueryResult(MemoryQueryResult memoryQueryResult)
    {
        var source = JsonSerializer.Deserialize<Dictionary<string, string>>(memoryQueryResult.Metadata.AdditionalMetadata).GetValueOrDefault(metadataSourceKey, "unknown");
        var text = $"{memoryQueryResult.Metadata.Text}\nINFORMATION SOURCE: {source}\n\n";

        var newMetadata = new MemoryRecordMetadata(
                memoryQueryResult.Metadata.IsReference,
                memoryQueryResult.Metadata.Id,
                text,
                memoryQueryResult.Metadata.Description,
                memoryQueryResult.Metadata.ExternalSourceName,
                memoryQueryResult.Metadata.AdditionalMetadata);

        return new MemoryQueryResult(newMetadata, memoryQueryResult.Relevance, memoryQueryResult.Embedding);
    }
}
