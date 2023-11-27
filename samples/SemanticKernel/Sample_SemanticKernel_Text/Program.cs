using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

using Sample_SemanticKernel_Text.Text;

internal sealed class Program
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
                    .WithAzureChatCompletionService(options.ChatModelDeploymentName, options.Endpoint.ToString(), options.Key)
                    .Build();

                kernel.ImportTextPlugin();

                return kernel;
            });
        });

        var host = hostBuilder.Build();

        // Initialize Examples
        var exampleClass = new MyClass(host.Services.GetService<IKernel>());

        await exampleClass.TestSummaryAsync();

        await exampleClass.TextKeyPhrasesAsync();
    }
}
