using Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace Encamina.Enmarcha.Samples.SemanticKernel.QuestionAnswering;

/// <summary>
/// Example class of Question Answering Plugin.
/// </summary>
internal class TestQuestionAnswering
{
    private readonly IKernel kernel;

    public TestQuestionAnswering(IKernel kernel)
    {
        this.kernel = kernel;
    }

    public async Task<string> TestQuestionAnsweringFromContextAsync()
    {
        var input = @"What year was the French Revolution?";
        var context = @"The French Revolution[a] was a period of radical political and societal change in France that began with the Estates General of 1789, 
and ended with the formation of the French Consulate in November 1799. Many of its ideas are considered fundamental principles of liberal democracy,
while the values and institutions it created remain central to French political discourse. Its causes are generally agreed to be a combination of social,
political and economic factors, which the Ancien Régime proved unable to manage. In May 1789, widespread social distress led to the convocation of the Estates General,
which was converted into a National Assembly in June. Continuing unrest culminated in the Storming of the Bastille on 14 July, which led to a series of radical 
measures by the Assembly, including the abolition of feudalism, the imposition of state control over the Catholic Church in France, and extension of the right 
to vote.";

        Console.WriteLine($"# Context: {context} \n");
        Console.WriteLine($"# Question: {input} \n");

        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Input, input);
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Parameters.Context, context);

        var functionQuestionAnswering = kernel.Functions.GetFunction(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromContext.Name);
        var resultContext = await kernel.RunAsync(contextVariables, functionQuestionAnswering);
        var result = resultContext.GetValue<string>();

        Console.WriteLine($"# Result: {result} \n");

        return result;
    }

    public async Task<string> TestQuestionAnsweringFromMemoryAsync()
    {
        var input = "What period occurred the Industrial Revolution?";
        var contextVariables = new ContextVariables();
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.Question, input);
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionSeparator, ",");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.CollectionsStr, "my-collection");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.MinRelevance, "0.8");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResultsLimit, "1");
        contextVariables.Set(PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Parameters.ResponseTokenLimit, "300");

        Console.WriteLine($"# Question: {input} \n");

        var functionQuestionAnswering = kernel.Functions.GetFunction(PluginsInfo.QuestionAnsweringPlugin.Name, PluginsInfo.QuestionAnsweringPlugin.Functions.QuestionAnsweringFromMemoryQuery.Name);
        var resultContext = await kernel.RunAsync(contextVariables, functionQuestionAnswering);
        var result = resultContext.GetValue<string>();

        Console.WriteLine($"# Result: {result} \n");

        return result;
    }
}
