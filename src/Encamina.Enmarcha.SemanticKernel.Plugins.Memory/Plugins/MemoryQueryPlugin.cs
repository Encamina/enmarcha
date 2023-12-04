using System.ComponentModel;
using System.Text;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Memory.Plugins;

/// <summary>
/// Plugin to query a memory.
/// </summary>
public class MemoryQueryPlugin
{
    private readonly ISemanticTextMemory semanticTextMemory;
    private readonly Func<string, int> tokenLengthFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryQueryPlugin"/> class.
    /// </summary>
    /// <param name="semanticTextMemory">A valid instance of a semantic memory to recall memories associated with text.</param>
    /// <param name="tokenLengthFunction">Function to calculate the length of a string in tokens.</param>
    public MemoryQueryPlugin(ISemanticTextMemory semanticTextMemory, Func<string, int> tokenLengthFunction)
    {
        this.semanticTextMemory = semanticTextMemory;
        this.tokenLengthFunction = tokenLengthFunction ?? throw new ArgumentNullException(nameof(tokenLengthFunction));
    }

    /// <summary>
    /// Searches for a query in a memory's collections.
    /// </summary>
    /// <param name="query">The query to look for in the memory's collections.</param>
    /// <param name="collectionsStr">A list of collections names, separated by the value of <paramref name="collectionSeparator"/> (usually a comma).</param>
    /// <param name="responseTokenLimit">Maximum number of tokens to use for the response.</param>
    /// <param name="minRelevance">Minimum relevance of the response.</param>
    /// <param name="resultsLimit">Maximum number of results from searching each memory's collection.</param>
    /// <param name="collectionSeparator">The character that separates each memory's collection name in <paramref name="collectionsStr"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A string representing all the information found from searching the memory's collections using the given <paramref name="query"/>.</returns>
    [SKFunction]
    [Description(@"Searches the memory by looking up for a given query, from a list (usually comma-separated) of in memory's collections .")]
    public virtual async Task<string> QueryMemoryAsync(
        [Description(@"The query to search the memory for")] string query,
        [Description(@"A list of collections, usually comma-separated")] string collectionsStr,
        [Description(@"Available maximum number of tokens for the response")] int responseTokenLimit,
        [Description(@"Minimum relevance for the search results")] double minRelevance = 0.75,
        [Description(@"Maximum number of results per queried collection")] int resultsLimit = 20,
        [Description(@"The character (usually a comma) that separates each collection from the given list of collections")] char collectionSeparator = ',',
        CancellationToken cancellationToken = default)
    {
        var relevantMemories = new List<MemoryQueryResult>();

        var collections = collectionsStr.Split(new[] { collectionSeparator }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var documentCollection in collections)
        {
            var results = semanticTextMemory.SearchAsync(
                documentCollection,
                query,
                limit: resultsLimit,
                minRelevanceScore: minRelevance,
                cancellationToken: cancellationToken);

            relevantMemories.AddRange(await results.ToListAsync(cancellationToken));
        }

        var memorySnippets = new StringBuilder();

        foreach (var memoryText in relevantMemories.OrderByDescending(m => m.Relevance).Select(m => m.Metadata.Text))
        {
            var memoryTokenCount = tokenLengthFunction(memoryText);

            if (responseTokenLimit - memoryTokenCount < 0)
            {
                break;
            }

            memorySnippets.AppendLine(memoryText);
            responseTokenLimit -= memoryTokenCount;
        }

        return memorySnippets.ToString();
    }
}

