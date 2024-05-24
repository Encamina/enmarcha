#pragma warning disable S2360 // Optional parameters should not be used

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Encamina.Enmarcha.Bot.Skills.QuestionAnswering;

using Encamina.Enmarcha.Core.Extensions;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using ExceptionMessages = Encamina.Enmarcha.Bot.Skills.QuestionAnswering.Resources.ExceptionMessages;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods to configure this skill.
/// </summary>
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures the question answering dialog with given configuration options and question answering service name.
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">An action with configuration options for the question answering dialog.</param>
    /// <param name="serviceLifetime">The lifetime for the default bot states.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">
    /// If the current collection of services do not have a valid <see cref="ICognitiveServiceFactory{IQuestionAnsweringService}">cognitive service factory for question
    /// answering service</see> registered.
    /// </exception>
    public static IServiceCollection AddQuestionAnsweringSkill(this IServiceCollection services, Action<QuestionAnsweringSkillOptions> options, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddQuestionAnsweringSkill(optionsBuilder => optionsBuilder.Configure(options), serviceLifetime);
    }

    /// <summary>
    /// Adds support for Cosmos DB with configuration parameters from the current configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The current set of key-value application configuration parameters.</param>
    /// <param name="serviceLifetime">The lifetime for the default bot states.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">
    /// If the current collection of services do not have a valid <see cref="ICognitiveServiceFactory{IQuestionAnsweringService}">cognitive service factory for question
    /// answering service</see> registered.
    /// </exception>
    public static IServiceCollection AddQuestionAnsweringSkill(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        return services.AddQuestionAnsweringSkill(optionsBuilder => optionsBuilder.Bind(configuration.GetSection(nameof(QuestionAnsweringSkillOptions))), serviceLifetime);
    }

    private static IServiceCollection AddQuestionAnsweringSkill(this IServiceCollection services, Func<OptionsBuilder<QuestionAnsweringSkillOptions>, OptionsBuilder<QuestionAnsweringSkillOptions>> configureOptions, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        if (!services.Any(s => s.ServiceType.IsAssignableFrom(typeof(ICognitiveServiceFactory<IQuestionAnsweringService>))))
        {
            throw new InvalidOperationException(ExceptionMessages.ResourceManager.GetStringByCurrentCulture(nameof(ExceptionMessages.MissingQuestionAnsweringCognitiveServiceFabric)));
        }

        var dialogId = Guid.NewGuid().ToString();

        configureOptions(services.AddOptions<QuestionAnsweringSkillOptions>(dialogId)).ValidateDataAnnotations().ValidateOnStart();

        var factory = new Func<IServiceProvider, QuestionAnsweringDialog>(sp =>
          new QuestionAnsweringDialog(dialogId, sp.GetRequiredService<QuestionAnsweringDialogServices>(), sp.GetRequiredService<IOptionsMonitor<QuestionAnsweringSkillOptions>>()));

        return services.AddType<QuestionAnsweringDialogServices>(serviceLifetime)
                       .AddType<Dialog, QuestionAnsweringDialog>(serviceLifetime, sp => factory(sp))
                       .AddType(serviceLifetime, sp => factory(sp));
    }
}

#pragma warning restore S2360 // Optional parameters should not be used
