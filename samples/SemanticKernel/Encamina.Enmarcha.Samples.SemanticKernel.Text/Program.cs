using Encamina.Enmarcha.AI.OpenAI.Azure;
using Encamina.Enmarcha.SemanticKernel.Plugins.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.SemanticKernel;

namespace Encamina.Enmarcha.Samples.SemanticKernel.Text;

internal static class Program
{
    private static async Task Main(string[] _)
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
            services.AddScoped(_ =>
            {
                // Get semantic kernel options
                var options = hostContext.Configuration.GetRequiredSection(nameof(AzureOpenAIOptions)).Get<AzureOpenAIOptions>()
                ?? throw new InvalidOperationException(@$"Missing configuration for {nameof(AzureOpenAIOptions)}");

                // Initialize semantic kernel
                var kernel = Kernel.CreateBuilder()
                    .AddAzureOpenAIChatCompletion(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                kernel.ImportTextPlugin();

                return kernel;
            });
        });

        var host = hostBuilder.Build();

        // Initialize Examples
        var example = new Example(host.Services.GetRequiredService<Kernel>());

        await example.TestSummaryAsync();

        await example.TextKeyPhrasesAsync();
    }
}
