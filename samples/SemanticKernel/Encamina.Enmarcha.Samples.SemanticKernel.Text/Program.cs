﻿using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.Text;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        // Create and configure builder
        var hostBuilder = new HostBuilder()
            .ConfigureAppConfiguration((configuration) =>
            {
                configuration.AddJsonFile(path: @"appsettings.json", optional: false, reloadOnChange: true);
            });

        // Configure service
        hostBuilder.ConfigureServices((hostContext, services) =>
        {
            services.AddScoped(sp =>
            {
                // Get semantic kernel options
                var options = hostContext.Configuration.GetRequiredSection(nameof(SemanticKernelOptions)).Get<SemanticKernelOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(SemanticKernelOptions)}");

                // Initialize semantic kernel
                var kernel = new KernelBuilder()
                    .WithAzureOpenAIChatCompletionService(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                kernel.ImportTextPlugin();

                return kernel;
            });
        });

        var host = hostBuilder.Build();

        // Initialize Examples
        var example = new Example(host.Services.GetRequiredService<IKernel>());

        await example.TestSummaryAsync();

        await example.TextKeyPhrasesAsync();
    }
}
