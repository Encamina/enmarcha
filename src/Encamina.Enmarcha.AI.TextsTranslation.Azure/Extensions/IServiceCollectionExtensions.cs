using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Azure;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure
/// <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/translator/text-translation-overview">Azure Translator Service</see>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for the <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/translator/text-translation-overview">Azure Translator Service</see>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static ITextTranslationServiceBuilder AddAzureTextTranslationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient()
                .AddOptions<TextTranslationConfigurations>().Bind(configuration.GetSection(nameof(TextTranslationConfigurations))).ValidateDataAnnotations().ValidateOnStart();

        services.AddSingleton<TextTranslationServiceFactory>()
                .AddSingleton<ICognitiveServiceFactory<TextTranslationService>>(serviceProvider => serviceProvider.GetRequiredService<TextTranslationServiceFactory>())
                .AddSingleton<ICognitiveServiceFactory<ITextTranslationService>>(serviceProvider => serviceProvider.GetRequiredService<TextTranslationServiceFactory>());

        return new TextTranslationServiceBuilder(services);
    }
}
