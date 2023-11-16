using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;
using Encamina.Enmarcha.AI.IntentsPrediction.Azure;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure
/// <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/overview">
/// Azure Conversational Language Understanding Service Orchestrations
/// </see>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for the <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/conversational-language-understanding/overview">
    /// Azure Conversational Language Understanding Service Orchestrations</see>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureIntentPredictionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<IntentPredictionConfigurations>().Bind(configuration.GetSection(nameof(IntentPredictionConfigurations))).ValidateDataAnnotations().ValidateOnStart();

        return services.AddSingleton<IntentPredictionServiceFactory>()
                       .AddSingleton<ICognitiveServiceFactory<IntentPredictionService>>(serviceProvider => serviceProvider.GetRequiredService<IntentPredictionServiceFactory>())
                       .AddSingleton<ICognitiveServiceFactory<IIntentPredictionService>>(serviceProvider => serviceProvider.GetRequiredService<IntentPredictionServiceFactory>());
    }
}
