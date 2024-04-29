using System.Diagnostics.CodeAnalysis;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions;
using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

namespace Encamina.Enmarcha.AI;

/// <summary>
/// Base class for any provider of cognitive services.
/// </summary>
public class CognitiveServiceProviderBase : ICognitiveServiceProvider
{
    private readonly ICognitiveServiceFactory<IConversationAnalysisService>? conversationAnalysisServiceFactory;
    private readonly ICognitiveServiceFactory<IIntentPredictionService>? intentPredictionServiceFactory;
    private readonly ICognitiveServiceFactory<ILanguageDetectionService>? languageDetectionServiceFactory;
    private readonly ICognitiveServiceFactory<IQuestionAnsweringService>? questionsAnsweringServiceFactory;
    private readonly ICognitiveServiceFactory<ITextTranslationService>? textTranslationServiceFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CognitiveServiceProviderBase"/> class.
    /// </summary>
    /// <param name="conversationAnalysisServiceFactory">A cognitive service factory for conversation analysis cognitive services.</param>
    /// <param name="intentPredictionServiceFactory">A cognitive service factory for intent prediction cognitive services.</param>
    /// <param name="languageDetectionServiceFactory">>A cognitive service factory for language detection cognitive services.</param>
    /// <param name="questionsAnsweringServiceFactory">>A cognitive service factory for questions answering cognitive services.</param>
    /// <param name="textTranslationServiceFactory">>A cognitive service factory for text translation cognitive services.</param>
    [SuppressMessage(@"Critical Code Smell", @"S2360:Optional parameters should not be used", Justification = @"Optional parameters allows supporting conditional dependency injection!")]
    protected CognitiveServiceProviderBase(
        ICognitiveServiceFactory<IConversationAnalysisService>? conversationAnalysisServiceFactory = null,
        ICognitiveServiceFactory<IIntentPredictionService>? intentPredictionServiceFactory = null,
        ICognitiveServiceFactory<ILanguageDetectionService>? languageDetectionServiceFactory = null,
        ICognitiveServiceFactory<IQuestionAnsweringService>? questionsAnsweringServiceFactory = null,
        ICognitiveServiceFactory<ITextTranslationService>? textTranslationServiceFactory = null)
    {
        this.conversationAnalysisServiceFactory = conversationAnalysisServiceFactory;
        this.intentPredictionServiceFactory = intentPredictionServiceFactory;
        this.languageDetectionServiceFactory = languageDetectionServiceFactory;
        this.questionsAnsweringServiceFactory = questionsAnsweringServiceFactory;
        this.textTranslationServiceFactory = textTranslationServiceFactory;
    }

    /// <inheritdoc/>
    public virtual IConversationAnalysisService GetConversationAnalysisService(string serviceName) => GetByName(conversationAnalysisServiceFactory, serviceName)!;

    /// <inheritdoc/>
    public virtual ILanguageDetectionService GetLanguageDetectionService(string serviceName) => GetByName(languageDetectionServiceFactory, serviceName)!;

    /// <inheritdoc/>
    public virtual IQuestionAnsweringService GetQuestionsAnsweringService(string serviceName) => GetByName(questionsAnsweringServiceFactory, serviceName)!;

    /// <inheritdoc/>
    public virtual ITextTranslationService GetTextTranslationService(string serviceName) => GetByName(textTranslationServiceFactory, serviceName)!;

    /// <inheritdoc/>
    public virtual IIntentPredictionService GetIntentPredictionService(string serviceName) => GetByName(intentPredictionServiceFactory, serviceName)!;

    /// <inheritdoc/>
    public bool TryGetConversationAnalysisService(string serviceName, out IConversationAnalysisService? service)
    {
        service = GetByName(conversationAnalysisServiceFactory, serviceName, false);

        return service != null;
    }

    /// <inheritdoc/>
    public bool TryGetLanguageDetectionService(string serviceName, out ILanguageDetectionService? service)
    {
        service = GetByName(languageDetectionServiceFactory, serviceName, false);

        return service != null;
    }

    /// <inheritdoc/>
    public bool TryGetQuestionsAnsweringService(string serviceName, out IQuestionAnsweringService? service)
    {
        service = GetByName(questionsAnsweringServiceFactory, serviceName, false);

        return service != null;
    }

    /// <inheritdoc/>
    public bool TryGetTextTranslationService(string serviceName, out ITextTranslationService? service)
    {
        service = GetByName(textTranslationServiceFactory, serviceName, false);

        return service != null;
    }

    /// <inheritdoc/>
    public bool TryGetIntentPredictionService(string serviceName, out IIntentPredictionService? service)
    {
        service = GetByName(intentPredictionServiceFactory, serviceName, false);

        return service != null;
    }

    private static TCognitiveService? GetByName<TCognitiveService>(ICognitiveServiceFactory<TCognitiveService>? cognitiveServiceFactory, string serviceName, bool throwIfNotFound = true)
            where TCognitiveService : class, ICognitiveService
    {
        return cognitiveServiceFactory != null
            ? cognitiveServiceFactory.GetByName(serviceName, throwIfNotFound)
            : throw new InvalidOperationException(string.Format(Resources.ExceptionMessages.NotConfiguredCognitiveServiceFactory, typeof(TCognitiveService).Name));
    }
}
