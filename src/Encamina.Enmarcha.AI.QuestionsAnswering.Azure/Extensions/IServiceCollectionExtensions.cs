using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Azure;
using Encamina.Enmarcha.AI.QuestionsAnswering.Azure.Metadata;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/overview">Azure Question Answering Service</see>.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for the <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/language-service/question-answering/overview">Azure Question Answering Service</see>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration"> The current set of key-value application configuration parameters.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddAzureQuestionAnsweringServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<QuestionAnsweringConfigurations>().Bind(configuration.GetSection(nameof(QuestionAnsweringConfigurations))).ValidateDataAnnotations().ValidateOnStart();

        return services.AddSingleton<QuestionAnsweringServiceFactory>()
                       .AddSingleton<ICognitiveServiceFactory<QuestionAnsweringService>>(serviceProvider => serviceProvider.GetRequiredService<QuestionAnsweringServiceFactory>())
                       .AddSingleton<ICognitiveServiceFactory<IQuestionAnsweringService>>(serviceProvider => serviceProvider.GetRequiredService<QuestionAnsweringServiceFactory>());
    }

    /// <summary>
    /// Adds the <see cref="CachedTableStorageCompositeMetadataHandler"/> as an available <see cref="IMetadataHandler"/> to process metadata.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">Action to configure options for the metadata handler.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddCachedTableStorageCompositeMetadataHandler(this IServiceCollection services, Action<CachedTableStorageCompositeMetadataHandlerOptions> options)
    {
        Guard.IsNotNull(services);
        Guard.IsNotNull(options);

        return services.Configure(options)
                       .AddSingleton<CachedTableStorageCompositeMetadataHandler>()
                       .AddSingleton<IMetadataHandler, CachedTableStorageCompositeMetadataHandler>();
    }
}
