using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Azure;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure
/// <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/language-detection/overview">Azure Language Detection Service</see>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for the <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/language-detection/overview">Azure Language Detection Service</see>
    /// powered by Text Analytics from Azure Cognitive Services for Language.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureTextAnalyticsLanguageDetectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TextAnalyticsLanguageDetectionConfigurations>().Bind(configuration.GetSection(nameof(TextAnalyticsLanguageDetectionConfigurations)))
                                                                           .ValidateDataAnnotations()
                                                                           .ValidateOnStart();

        return services.AddSingleton<TextAnalyticsLanguageDetectionServiceFactory>()
                       .AddSingleton<ICognitiveServiceFactory<TextAnalyticsLanguageDetectionService>>(sp => sp.GetRequiredService<TextAnalyticsLanguageDetectionServiceFactory>())
                       .AddSingleton<ICognitiveServiceFactory<ILanguageDetectionService>>(sp => sp.GetRequiredService<TextAnalyticsLanguageDetectionServiceFactory>());
    }

    /// <summary>
    /// Adds support for the <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-detect">Azure Translator Language Detection</see>
    /// powered by Azure Translator Service.
    /// </summary>
    /// <remarks>This implementation currently uses version 3.0.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name = "configuration" > The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureTranslatorLanguageDetectionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient()
                .AddOptions<TranslatorLanguageDetectionConfigurations>()
                .Bind(configuration.GetSection(nameof(TranslatorLanguageDetectionConfigurations)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        return services.AddSingleton<TranslatorLanguageDetectionServiceFactory>()
                       .AddSingleton<ICognitiveServiceFactory<TranslatorLanguageDetectionService>>(sp => sp.GetRequiredService<TranslatorLanguageDetectionServiceFactory>())
                       .AddSingleton<ICognitiveServiceFactory<ILanguageDetectionService>>(sp => sp.GetRequiredService<TranslatorLanguageDetectionServiceFactory>());
    }
}
