using System.ComponentModel;
using System.Globalization;
using System.Reflection;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Extensions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.Plugins;

/// <summary>
/// Represents a plugin that allows users to ask questions with information retrieved from querying a memory.
/// </summary>
public class QuestionAnsweringPlugin
{
    private readonly IKernel kernel;
    private readonly string modelName;
    private readonly Func<string, int> tokenLengthFunction;

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringPlugin"/> class.
    /// </summary>
    /// <param name="kernel">The instance of the semantic kernel to work with in this plugin.</param>
    /// <param name="modelName">The name of the model used by this plugin.</param>
    /// <param name="tokensLengthFunction">A function to count how many tokens are in a string or text.</param>
    public QuestionAnsweringPlugin(IKernel kernel, string modelName, Func<string, int> tokensLengthFunction)
    {
        this.kernel = kernel;
        this.modelName = modelName;
        this.tokenLengthFunction = tokensLengthFunction;
    }

    /// <summary>
    /// Answer questions using information obtained from a memory. The given question is used as query to search from a list (usually comma-separated) of collections.
    /// The result is used as context to answer the question.
    /// </summary>
    /// <param name="question">The question to answer and search the memory for.</param>
    /// <param name="collectionsStr">A list of collections names, separated by the value of <paramref name="collectionSeparator"/> (usually a comma).</param>
    /// <param name="responseTokenLimit">Maximum number of tokens to use for the response.</param>
    /// <param name="minRelevance">Minimum relevance of the response.</param>
    /// <param name="resultsLimit">Maximum number of results from searching each memory's collection.</param>
    /// <param name="collectionSeparator">The character that separates each memory's collection name in <paramref name="collectionsStr"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A string representing the answer for the <paramref name="question"/> based on all the information found from searching the memory's collections.</returns>
    [SKFunction]
    [Description(@"Answer questions using information obtained from a memory. The given question is used as query to search from a list (usually comma-separated) of collections. The result is used as context to answer the question.")]
    public virtual async Task<string> QuestionAnsweringFromMemoryQuery(
        [Description(@"The question to answer and search the memory for")] string question,
        [Description(@"A list of memory's collections, usually comma-separated")] string collectionsStr,
        [Description(@"Available maximum number of tokens for the answer")] int responseTokenLimit,
        [Description(@"Minimum relevance for the search results")] double minRelevance = 0.75,
        [Description(@"Maximum number of results per queried collection")] int resultsLimit = 20,
        [Description(@"The character (usually a comma) that separates each collection from the given list of collections")] char collectionSeparator = ',',
        CancellationToken cancellationToken = default)
    {
        var modelMaxTokens = ModelInfo.GetById(modelName).MaxTokens; // Get this information early, to throw an exception if the model is not found (fail fast).

        // This method was designed to maximize the use of tokens in an LLM model (like GPT).
        // First, it calculates the number of tokens in the 'prompt', 'input', and 'output' of the «QuestionAnsweringFromContext» function. (First context)
        // Then, uses a new context to call the «QueryMemory» function from the «MemoryQueryPlugin». (Second context)
        // Then, it subtracts this value from the total tokens of the model to determine how many tokens can be used for the memory query. (Second context)
        // Finally, it injects the result of the memory query into the first context so that the response function can use it. (First context)

        var questionAnsweringVariables = new ContextVariables();
        questionAnsweringVariables.Set(@"input", question);

        var questionAnsweringFunction = kernel.Skills.GetFunction(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Name);

        // Calculates the number of tokens used in the «QuestionAnsweringFromContext» function.
        // This amount will be subtracted from the total tokens of the model to determine the token limit required by the «QueryMemory» funtion from the «MemoryQueryPlugin».
        var questionAnsweringFunctionUsedTokens
            = await kernel.GetSemanticFunctionUsedTokensAsync(questionAnsweringFunction, Assembly.GetExecutingAssembly(), questionAnsweringVariables, tokenLengthFunction, cancellationToken);

        // Switches to the context of the memory query function.
        var memoryQueryVariables = new ContextVariables();
        memoryQueryVariables.Set(@"query", question);
        memoryQueryVariables.Set(@"collectionsStr", collectionsStr);
        memoryQueryVariables.Set(@"responseTokenLimit", (modelMaxTokens - questionAnsweringFunctionUsedTokens).ToString(CultureInfo.InvariantCulture));
        memoryQueryVariables.Set(@"minRelevance", minRelevance.ToString(CultureInfo.InvariantCulture));
        memoryQueryVariables.Set(@"resultsLimit", resultsLimit.ToString(CultureInfo.InvariantCulture));
        memoryQueryVariables.Set(@"collectionSeparator", collectionSeparator.ToString(CultureInfo.InvariantCulture));

        // Executes the «QueryMemory» function from the «MemoryQueryPlugin»
        var memoryQueryFunction = kernel.Skills.GetFunction(Memory.PluginsInfo.MemoryQueryPlugin.Name, Memory.PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Name);
        var memoryQueryResultContext = await kernel.RunAsync(memoryQueryVariables, cancellationToken, memoryQueryFunction);
        memoryQueryResultContext.ValidateAndThrowIfErrorOccurred();

        // If the «QueryMemory» function from the «MemoryQueryPlugin» does not return any result, there is no point in trying to responsd. In such a case, `null` is returned.
        if (string.IsNullOrWhiteSpace(memoryQueryResultContext.Result))
        {
            return null;
        }

        // Return to the context of the response function and set the result of the memory query.
        questionAnsweringVariables.Set(@"context", memoryQueryResultContext.Result);
        var questionAnsweringResultContext = await kernel.RunAsync(questionAnsweringVariables, cancellationToken, questionAnsweringFunction);
        questionAnsweringResultContext.ValidateAndThrowIfErrorOccurred();

        return questionAnsweringResultContext.Result;
    }
}
