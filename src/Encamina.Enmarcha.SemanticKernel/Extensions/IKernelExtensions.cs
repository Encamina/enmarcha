﻿using System.Reflection;
using System.Text.Json;

using Encamina.Enmarcha.Core.Extensions;

using Encamina.Enmarcha.SemanticKernel.Extensions.Resources;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;
using Microsoft.SemanticKernel.TemplateEngine.Basic;

namespace Encamina.Enmarcha.SemanticKernel.Extensions;

/// <summary>
/// Extension methods on <see cref="IKernel"/>.
/// </summary>
public static class IKernelExtensions
{
    /// <summary>
    /// Generates the final prompt for a given semantic function in a directory located plugin, and using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> to work with.</param>
    /// <param name="skFunction">The semantic function representation.</param>
    /// <param name="functionPluginDirectory">The directory containing the plugin and the files that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A string containing the generated prompt.</returns>
    public static async Task<string> GetSemanticFunctionPromptAsync(this IKernel kernel, ISKFunction skFunction, string functionPluginDirectory, IDictionary<string, string> contextVariables, CancellationToken cancellationToken)
    {
        var kernelContext = kernel.CreateNewContext();

        foreach (var (key, value) in contextVariables)
        {
            kernelContext.Variables[key] = value;
        }

        var promptTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, functionPluginDirectory, skFunction.SkillName, skFunction.Name, Constants.PromptFile);

        return File.Exists(promptTemplatePath)
            ? await kernel.PromptTemplateEngine.RenderAsync(await File.ReadAllTextAsync(promptTemplatePath, cancellationToken), kernelContext, cancellationToken)
            : throw new FileNotFoundException(ExceptionMessages.ResourceManager.GetFormattedStringByCurrentCulture(nameof(ExceptionMessages.PromptFileNotFound), skFunction.Name, skFunction.SkillName, functionPluginDirectory));
    }

    /// <summary>
    /// Generates the final prompt for a given semantic function from embedded resources in an assembly, using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> to work with.</param>
    /// <param name="skFunction">The semantic function representation.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A string containing the generated prompt.</returns>
    public static async Task<string> GetSemanticFunctionPromptAsync(this IKernel kernel, ISKFunction skFunction, Assembly assembly, IDictionary<string, string> contextVariables, CancellationToken cancellationToken)
    {
        var kernelContext = kernel.CreateNewContext();

        foreach (var (key, value) in contextVariables)
        {
            kernelContext.Variables[key] = value;
        }

        var resourceNames = assembly.GetManifestResourceNames()
                                    .Where(resourceName => resourceName.IndexOf($"{skFunction.SkillName}.{skFunction.Name}", StringComparison.OrdinalIgnoreCase) != -1)
                                    .ToList(); // Enumerate here to improve performance.

        var promptConfigurationResourceName = resourceNames.SingleOrDefault(resourceName => resourceName.EndsWith(Constants.ConfigFile, StringComparison.OrdinalIgnoreCase));
        var promptTemplateResourceName = resourceNames.SingleOrDefault(resourceName => resourceName.EndsWith(Constants.PromptFile, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(promptConfigurationResourceName) || string.IsNullOrEmpty(promptTemplateResourceName))
        {
            return null;
        }

        // TODO : Check this once the final version of Semantic Kernel is released (it seems that it will be changed by `KernelPromptTemplateFactory`)...
        var promptTemplateConfig = PromptTemplateConfig.FromJson(await ReadResourceAsync(assembly, promptConfigurationResourceName));
        var promptTemplate = new BasicPromptTemplateFactory(kernel.LoggerFactory).Create(await ReadResourceAsync(assembly, promptTemplateResourceName), promptTemplateConfig);

        return await promptTemplate.RenderAsync(kernelContext, cancellationToken);
    }

    /// <summary>
    /// Calculates the current total number of tokens used in generating a prompt of a given semantic function in a directory located plugin, and using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> to work with.</param>
    /// <param name="skFunction">The semantic function representation.</param>
    /// <param name="functionPluginDirectory">The directory containing the plugin and the files that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="tokenLengthFunction">A function to calculate length of a string in tokens..</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The total number of tokens used plus the maximum allowed response tokens specified in the function.</returns>
    public static async Task<int> GetSemanticFunctionUsedTokensAsync(this IKernel kernel, ISKFunction skFunction, string functionPluginDirectory, IDictionary<string, string> contextVariables, Func<string, int> tokenLengthFunction, CancellationToken cancellationToken)
    {
        return tokenLengthFunction(await kernel.GetSemanticFunctionPromptAsync(skFunction, functionPluginDirectory, contextVariables, cancellationToken)) + GetMaxTokensFrom(skFunction);
    }

    /// <summary>
    /// Calculates the current total number of tokens used in generating a prompt of a given semantic function from embedded resources in an assembly, using the context variables.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> to work with.</param>
    /// <param name="skFunction">The semantic function representation.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <param name="contextVariables">A collection of context variables.</param>
    /// <param name="tokenLengthFunction">A function to calculate length of a string in tokens..</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>The total number of tokens used plus the maximum allowed response tokens specified in the function.</returns>
    public static async Task<int> GetSemanticFunctionUsedTokensAsync(this IKernel kernel, ISKFunction skFunction, Assembly assembly, IDictionary<string, string> contextVariables, Func<string, int> tokenLengthFunction, CancellationToken cancellationToken)
    {
        return tokenLengthFunction(await kernel.GetSemanticFunctionPromptAsync(skFunction, assembly, contextVariables, cancellationToken)) + GetMaxTokensFrom(skFunction);
    }

    /// <summary>
    /// Imports plugins with semantic functions from embedded resources in an assembly that represents their prompt and configuration files.
    /// </summary>
    /// <param name="kernel">The <see cref="IKernel"/> to work with.</param>
    /// <param name="assembly">The assembly containing the embedded resources that represents and configures the semantic function.</param>
    /// <returns>A list of all the semantic functions found in the assembly, indexed by function name.</returns>
    public static IDictionary<string, ISKFunction> ImportSemanticPluginsFromAssembly(this IKernel kernel, Assembly assembly)
    {
        var plugins = new Dictionary<string, ISKFunction>();

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
                                        .GroupBy(x => (x.PluginName, x.FunctionName), x => (x.ResourceName, x.FileName)) // Group by skill and function names to get all the resources (prompt and configuration) for each function.
                                        ;

        foreach (var pluginsInfoGroup in pluginsInfoGroups)
        {
            var functionConfigResourceName = GetResourceNameFromPluginInfoByFileName(pluginsInfoGroup, Constants.ConfigFile);
            var functionPromptResourceName = GetResourceNameFromPluginInfoByFileName(pluginsInfoGroup, Constants.PromptFile);

            var promptTemplateConfig = PromptTemplateConfig.FromJson(ReadResource(assembly, functionConfigResourceName));

            // TODO : Check this once the final version of Semantic Kernel is released (it seems that it will be changed by `KernelPromptTemplateFactory`)...
            var promptTemplate = new BasicPromptTemplateFactory(kernel.LoggerFactory).Create(ReadResource(assembly, functionPromptResourceName), promptTemplateConfig);

            var (pluginName, functionName) = pluginsInfoGroup.Key;

            plugins[functionName] = kernel.RegisterSemanticFunction(pluginName, functionName, promptTemplateConfig, promptTemplate);
        }

        return plugins;
    }

    private static int GetMaxTokensFrom(ISKFunction sKFunction)
    {
        return sKFunction.RequestSettings.ExtensionData.TryGetValue(@"max_tokens", out var maxTokensObj) && maxTokensObj is JsonElement maxTokensElement && maxTokensElement.TryGetInt32(out var value) ? value : 0;
    }

    private static string ReadResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    private static async Task<string> ReadResourceAsync(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var streamReader = new StreamReader(stream);

        return await streamReader.ReadToEndAsync();
    }

    private static string GetResourceNameFromPluginInfoByFileName(IGrouping<(string PluginName, string FunctionName), (string ResourceName, string FileName)> pluginsInfoGroup, string fileName)
        => pluginsInfoGroup.Single(x => fileName.Equals(x.FileName, StringComparison.OrdinalIgnoreCase)).ResourceName;
}
