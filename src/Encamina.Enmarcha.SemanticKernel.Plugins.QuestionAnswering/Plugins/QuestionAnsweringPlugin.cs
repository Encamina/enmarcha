using System.ComponentModel;

using Encamina.Enmarcha.AI.OpenAI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Extensions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.Plugins;

/// <summary>
/// Represents a plugin that allows users to ask questions with information retrieved from querying a memory.
/// </summary>
public class QuestionAnsweringPlugin
{
    private const string QuestionAnsweringFromContextFunctionPrompt = @"
You ANSWER questions with information from the CONTEXT.
ONLY USE information from CONTEXT
The ANSWER MUST BE ALWAYS in the SAME LANGUAGE as the QUESTION. 
If you are unable to find the answer or do not know it, simply say ""I don't know"". 
The ""I don't know"" response MUST BE TRANSLATED ALWAYS to the SAME LANGUAGE as the QUESTION. 
If presented with a logic question about the CONTEXT, attempt to calculate the answer. 
ALWAYS RESPOND with a FINAL ANSWER, DO NOT CONTINUE the conversation.

[CONTEXT]
{{$context}}

[QUESTION]
{{$input}}

[ANSWER]

";

    private readonly Kernel kernel;
    private readonly string modelName;
    private readonly Func<string, int> tokenLengthFunction;
    private readonly OpenAIPromptExecutionSettings questionAnsweringFromContextFunctionExecutionSettings = new()
    {
        MaxTokens = 1000,
        Temperature = 0.1,
        TopP = 1.0,
        PresencePenalty = 0.0,
        FrequencyPenalty = 0.0,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringPlugin"/> class.
    /// </summary>
    /// <param name="kernel">The instance of the semantic kernel to work with in this plugin.</param>
    /// <param name="modelName">The name of the model used by this plugin.</param>
    /// <param name="tokensLengthFunction">A function to count how many tokens are in a string or text.</param>
    public QuestionAnsweringPlugin(Kernel kernel, string modelName, Func<string, int> tokensLengthFunction)
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
    /// <param name="minRelevance">Minimum relevance of the response.</param>
    /// <param name="resultsLimit">Maximum number of results from searching each memory's collection.</param>
    /// <param name="collectionSeparator">The character that separates each memory's collection name in <paramref name="collectionsStr"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A string representing the answer for the <paramref name="question"/> based on all the information found from searching the memory's collections.</returns>
    [KernelFunction]
    [Description(@"Answer questions using information obtained from a memory. The given question is used as query to search from a list (usually comma-separated) of collections. The result is used as context to answer the question.")]
    public virtual async Task<string> QuestionAnsweringFromMemoryQueryAsync(
        [Description(@"The question to answer and search the memory for")] string question,
        [Description(@"A list of memory's collections, usually comma-separated")] string collectionsStr,
        [Description(@"Minimum relevance for the search results")] double minRelevance = 0.75,
        [Description(@"Maximum number of results per queried collection")] int resultsLimit = 20,
        [Description(@"The character (usually a comma) that separates each collection from the given list of collections")] char collectionSeparator = ',',
        CancellationToken cancellationToken = default)
    {
        // This method was designed to maximize the use of tokens in an LLM model (like GPT).
        // Get the number of tokens from the 'prompt', 'input' (question), and execution settings of the «QuestionAnsweringFromContext» function.
        // This number of tokens will be subtracted from the total tokens of the used model to determine the limit of tokens allowed for the «QueryMemory» function from the «MemoryQueryPlugin».
        // Finally, it uses the result from the «QueryMemory» function as context for the «QuestionAnsweringFromContext» function. The total amount of tokens of this context should be within the
        // limits of the available tokens for the context argument of the «QuestionAnsweringFromContext» function.

        var modelMaxTokens = ModelInfo.GetById(modelName).MaxTokens; // Get this information early, to throw an exception if the model is not found (fail fast).
        var questionAnsweringFunction = kernel.Plugins[PluginsInfo.QuestionAnsweringPlugin.Name][PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Name];

        var memoryQueryVariables = new KernelArguments()
        {
            [@"query"] = question,
            [@"collectionsStr"] = collectionsStr,
            [@"responseTokenLimit"] = modelMaxTokens - await GetQuestionAnsweringFromContextFunctionUsedTokensAsync(questionAnsweringFunction, question, cancellationToken),
            [@"minRelevance"] = minRelevance,
            [@"resultsLimit"] = resultsLimit,
            [@"collectionSeparator"] = collectionSeparator,
        };

        // Executes the «QueryMemory» function from the «MemoryQueryPlugin».
        var memoryQueryFunction = kernel.Plugins[Memory.PluginsInfo.MemoryQueryPlugin.Name][Memory.PluginsInfo.MemoryQueryPlugin.Functions.QueryMemory.Name];

        var memoryQueryFunctionResult = await memoryQueryFunction.InvokeAsync(kernel, memoryQueryVariables, cancellationToken);

        var memoryQueryResult = memoryQueryFunctionResult.GetValue<string>();

        // If the «QueryMemory» function from the «MemoryQueryPlugin» does not return any result, there is no point in trying to answering the question. In such a case, `null` is returned.
        if (string.IsNullOrWhiteSpace(memoryQueryResult))
        {
            return null;
        }

        // Return to the context of the response function and set the result of the memory query.
        var questionAnsweringVariables = new KernelArguments()
        {
            [@"input"] = question,
            [@"context"] = memoryQueryResult,
        };

        var questionAnsweringFunctionResult = await questionAnsweringFunction.InvokeAsync(kernel, questionAnsweringVariables, cancellationToken);

        return questionAnsweringFunctionResult.GetValue<string>();
    }

    /// <summary>
    /// Answer questions using information from a given context.
    /// </summary>
    /// <param name="input">The question to answer with information from a context given in <paramref name="context"/>.</param>
    /// <param name="context">The context with information that may contain the answer for question from <paramref name="input"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests.</param>
    /// <returns>A string representing the answer for the <paramref name="input"/> based on all the information found from searching the memory's collections.</returns>
    [KernelFunction]
    [Description(@"Answer questions using information from a context.")]
    public virtual async Task<string> QuestionAnsweringFromContextAsync(
        [Description(@"The question to answer with information from a context.")] string input,
        [Description(@"Context with information that may contain the answer for question")] string context,
        CancellationToken cancellationToken = default)
    {
        var functionArguments = new KernelArguments(questionAnsweringFromContextFunctionExecutionSettings)
        {
            [@"input"] = input,
            [@"context"] = context,
        };

        var functionResult = await kernel.InvokePromptAsync(QuestionAnsweringFromContextFunctionPrompt, functionArguments, cancellationToken: cancellationToken);

        return functionResult.GetValue<string>();
    }

    private Task<int> GetQuestionAnsweringFromContextFunctionUsedTokensAsync(KernelFunction questionAnsweringFromContextFunction, string input, CancellationToken cancellationToken)
    {
        var functionArguments = new KernelArguments(questionAnsweringFromContextFunctionExecutionSettings)
        {
            [@"input"] = input,
        };

        return kernel.GetKernelFunctionUsedTokensFromPromptAsync(QuestionAnsweringFromContextFunctionPrompt, questionAnsweringFromContextFunction, functionArguments, tokenLengthFunction, cancellationToken: cancellationToken);
    }
}
