using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.Plugins;

/// <summary>
/// Represents a plugin that allows users to ask questions with information retrieved from querying a memory.
/// Also, it adds the source of the information to the result.
/// </summary>
public class QuestionAnsweringWithSourcePlugin : QuestionAnsweringPlugin
{

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringWithSourcePlugin"/> class.
    /// </summary>
    /// <param name="kernel">The instance of the semantic kernel to work with in this plugin.</param>
    /// <param name="modelName">The name of the model used by this plugin.</param>
    /// <param name="tokensLengthFunction">A function to count how many tokens are in a string or text.</param>
    public QuestionAnsweringWithSourcePlugin(Kernel kernel, string modelName, Func<string, int> tokensLengthFunction) : base(kernel, modelName, tokensLengthFunction)
    {
    }

    /// <inheritdoc/>
    protected override string AdditionalInstructions => @"8. ALWAYS includes a reference to the INFORMATION SOURCE (for example, in English ""Sources:..."", in Spanish "" Fuentes:..."").";
}
