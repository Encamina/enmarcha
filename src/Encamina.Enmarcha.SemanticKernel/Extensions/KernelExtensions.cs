﻿using System.Reflection;
using System.Text.Json;

using Encamina.Enmarcha.Core.Extensions;

using Encamina.Enmarcha.SemanticKernel.Extensions.Resources;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.TextGeneration;

namespace Encamina.Enmarcha.SemanticKernel.Extensions;

/// <summary>
/// Extension methods on <see cref="Kernel"/>.
/// </summary>
public static class KernelExtensions
{
    /// <summary>
    /// Generates the final prompt for a given semantic function in a directory located plugin, and using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> to work with.</param>
    /// <param name="function">The kernel function.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A string containing the generated prompt.</returns>
    public static async Task<string> GetKernelFunctionPromptAsync(this Kernel kernel, string pluginDirectory, KernelFunction function, KernelArguments arguments, IPromptTemplateFactory promptTemplateFactory = null, CancellationToken cancellationToken = default)
    {
        var pluginName = function.Metadata.PluginName;

        var promptConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginDirectory, pluginName, function.Name, Constants.ConfigFile);

        if (!File.Exists(promptConfigPath))
        {
            throw new FileNotFoundException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ExceptionMessages.PromptConfigurationFileNotFound), function.Name, pluginName, pluginDirectory));
        }

        var promptTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginDirectory, pluginName, function.Name, Constants.PromptFile);

        if (!File.Exists(promptTemplatePath))
        {
            throw new FileNotFoundException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ExceptionMessages.PromptTemplateFileNotFound), function.Name, pluginName, pluginDirectory));
        }

        return await InnerGetKernelFunctionPromptAsync(kernel, arguments, function.Name, await File.ReadAllTextAsync(promptTemplatePath, cancellationToken), await File.ReadAllTextAsync(promptConfigPath, cancellationToken), promptTemplateFactory, cancellationToken);
    }

    /// <summary>
    /// Generates the final prompt for a given semantic function from embedded resources in an assembly, using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> to work with.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <param name="function">The kernel function.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A string containing the generated prompt.</returns>
    public static async Task<string> GetKernelFunctionPromptAsync(this Kernel kernel, Assembly assembly, KernelFunction function, KernelArguments arguments, IPromptTemplateFactory promptTemplateFactory = null, CancellationToken cancellationToken = default)
    {
        var pluginName = function.Metadata.PluginName;

        var resourceNames = assembly.GetManifestResourceNames()
                                    .Where(resourceName => resourceName.Contains($"{pluginName}.{function.Name}", StringComparison.OrdinalIgnoreCase))
                                    .ToList(); // Enumerate here to improve performance.

        var promptConfigurationResourceName = resourceNames.SingleOrDefault(resourceName => resourceName.EndsWith(Constants.ConfigFile, StringComparison.OrdinalIgnoreCase));
        var promptTemplateResourceName = resourceNames.SingleOrDefault(resourceName => resourceName.EndsWith(Constants.PromptFile, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(promptConfigurationResourceName) || string.IsNullOrEmpty(promptTemplateResourceName))
        {
            return null;
        }

        return await InnerGetKernelFunctionPromptAsync(kernel, arguments, function.Name, ReadResource(assembly, promptTemplateResourceName), ReadResource(assembly, promptConfigurationResourceName), promptTemplateFactory, cancellationToken);
    }

    /// <summary>
    /// Calculates the current total number of tokens used in generating a prompt of a given semantic function in a directory located plugin, and using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> to work with.</param>
    /// <param name="kernelFunction">The kernel function representation.</param>
    /// <param name="functionPluginDirectory">The directory containing the plugin and the files that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="tokenLengthFunction">A function to calculate length of a string in tokens..</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The total number of tokens used plus the maximum allowed response tokens specified in the function.</returns>
    public static async Task<int> GetKernelFunctionUsedTokensAsync(this Kernel kernel, string pluginDirectory, KernelFunction function, KernelArguments arguments, Func<string, int> tokenLengthFunction, IPromptTemplateFactory promptTemplateFactory = null, CancellationToken cancellationToken = default)
    {
        return tokenLengthFunction(await kernel.GetKernelFunctionPromptAsync(pluginDirectory, function, arguments, promptTemplateFactory, cancellationToken)) + GetMaxTokensFromKernelFunction(kernel, function);
    }

    /// <summary>
    /// Calculates the current total number of tokens used in generating a prompt of a given semantic function from embedded resources in an assembly, using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> to work with.</param>
    /// <param name="kernelFunction">The kernel function representation.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="tokenLengthFunction">A function to calculate length of a string in tokens..</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The total number of tokens used plus the maximum allowed response tokens specified in the function.</returns>
    public static async Task<int> GetKernelFunctionUsedTokensAsync(this Kernel kernel, Assembly assembly, KernelFunction function, KernelArguments arguments, Func<string, int> tokenLengthFunction, IPromptTemplateFactory promptTemplateFactory = null, CancellationToken cancellationToken = default)
    {
        return tokenLengthFunction(await kernel.GetKernelFunctionPromptAsync(assembly, function, arguments, promptTemplateFactory, cancellationToken)) + GetMaxTokensFromKernelFunction(kernel, function);
    }

    /// <summary>
    /// Imports plugins with semantic functions from embedded resources in an assembly that represents their prompt and configuration files.
    /// </summary>
    /// <param name="kernel">The <see cref="Kernel"/> to work with.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <param name="promptTemplateFactory">
    /// The <see cref="IPromptTemplateFactory"/> to use when interpreting discovered prompts into <see cref="IPromptTemplate"/>s.
    /// If <see langword="null"/>, a default factory will be used.
    /// </param>
    /// <returns>A list of all the semantic functions found in the assembly, indexed by function name.</returns>
    public static IEnumerable<KernelPlugin> ImportPromptFunctionsFromAssembly(this Kernel kernel, Assembly assembly, IPromptTemplateFactory promptTemplateFactory = null, ILoggerFactory loggerFactory = null)
    {
        var plugins = new List<KernelPlugin>();

        var pluginsInfoGroups = assembly.GetManifestResourceNames()
                                        .Where(resourceName => resourceName.EndsWith(Constants.ConfigFile, StringComparison.OrdinalIgnoreCase) || resourceName.EndsWith(Constants.PromptFile, StringComparison.OrdinalIgnoreCase))
                                        .Select(resourceName =>
                                        {
                                            var resourceNameTokens = resourceName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                                            // Initially, We do not know in which positions of the embedded resource's name is the name of the plugin and the name of each of its functions.
                                            // We know that the end of the embedded resource's name is either `config.json` or `skprompt.txt`, so we can work backwards from there to find this information.
                                            // So, the last token is the extension (either `.txt` or `.json`), then comes the file name (either `config` or `skprompt`), next is the name of the function,
                                            // and finally the name of the plugin.
                                            // Example: «xxxx.yyyyy.zzzzz.[SkillName].[FunctionName].config.json» and «xxxx.yyyyy.zzzzz.[SkillName].[FunctionName].skprompt.txt»
                                            return new
                                            {
                                                FileName = $@"{resourceNameTokens[^2]}.{resourceNameTokens[^1]}", // The file name and its extension are the last two tokens (first and second position from the end).
                                                FunctionName = resourceNameTokens[^3], // Next always comes the name of the function, which is in the third position from the end.
                                                PluginName = resourceNameTokens[^4],  // Finally comes the name of the plugin, which is in the fourth position from the end.
                                                ResourceName = resourceName,
                                            };
                                        })
                                        .GroupBy(x => (x.PluginName, x.FunctionName), x => (x.ResourceName, x.FileName)) // Group by skill and function names to get all the resources (prompt and configuration) for each function in a plugin.
                                        .GroupBy(x => x.Key.PluginName, x => x) // Then, group by plugin name to get all the functions in that plugin.
                                        ;

        var innerLoggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        var factory = promptTemplateFactory ?? new KernelPromptTemplateFactory(innerLoggerFactory ?? NullLoggerFactory.Instance);

        foreach (var pluginsInfoGroup in pluginsInfoGroups)
        {
            var functions = new List<KernelFunction>();

            foreach (var functionInfoGroup in pluginsInfoGroup)
            {
                var functionConfigResourceName = GetResourceNameFromPluginInfoByFileName(functionInfoGroup, Constants.ConfigFile);
                var functionPromptResourceName = GetResourceNameFromPluginInfoByFileName(functionInfoGroup, Constants.PromptFile);

                var promptConfig = PromptTemplateConfig.FromJson(ReadResource(assembly, functionConfigResourceName));
                promptConfig.Name = functionInfoGroup.Key.FunctionName;
                promptConfig.Template = ReadResource(assembly, functionPromptResourceName);

                var promptTemplateInstance = factory.Create(promptConfig);

                functions.Add(KernelFunctionFactory.CreateFromPrompt(promptTemplateInstance, promptConfig, innerLoggerFactory));
            }

            plugins.Add(KernelPluginFactory.CreateFromFunctions(pluginsInfoGroup.Key, functions));
        }

        return plugins;
    }

    private static int GetMaxTokensFromKernelFunction(Kernel kernel, KernelFunction kernelFunction)
    {
        // Try to use IChatCompletionService as the IAService to retrieve the service settings, but fallback to ITextGenerationService if it's not available.
        // Once the service settings are retrieved, get the value of the `max_tokens` property from the extension data.
        // If the `max_tokens` property is found, check is a JsonElement and try to get its value as an integer.
        // Finally, if the value is an integer, return it.
        // In any other case (if the service settings are not found, or the `max_tokens` property is not found, or the value is not an integer), throw an exception.
        return (kernel.ServiceSelector.TrySelectAIService<IChatCompletionService>(kernel, kernelFunction, [], out _, out var serviceSettings) || kernel.ServiceSelector.TrySelectAIService<ITextGenerationService>(kernel, kernelFunction, [], out _, out serviceSettings))
               && serviceSettings.ExtensionData.TryGetValue(@"max_tokens", out var maxTokensObj)
               && maxTokensObj is JsonElement maxTokensElement
               && maxTokensElement.TryGetInt32(out var value)
                ? value
                : throw new InvalidOperationException(@"Could not find in the kernel function execution settings with");
    }

    private static string GetResourceNameFromPluginInfoByFileName(IGrouping<(string PluginName, string FunctionName), (string ResourceName, string FileName)> pluginsInfoGroup, string fileName)
        => pluginsInfoGroup.Single(x => fileName.Equals(x.FileName, StringComparison.OrdinalIgnoreCase)).ResourceName;

    private static async Task<string> InnerGetKernelFunctionPromptAsync(Kernel kernel, KernelArguments arguments, string functionName, string promptTemplate, string promptConfigJsonString, IPromptTemplateFactory promptTemplateFactory = null, CancellationToken cancellationToken = default)
    {
        var promptConfig = PromptTemplateConfig.FromJson(promptConfigJsonString);
        promptConfig.Name = functionName;
        promptConfig.Template = promptTemplate;

        var factory = promptTemplateFactory ?? new KernelPromptTemplateFactory(NullLoggerFactory.Instance);

        return await factory.Create(promptConfig).RenderAsync(kernel, arguments, cancellationToken);
    }

    private static string ReadResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }
}