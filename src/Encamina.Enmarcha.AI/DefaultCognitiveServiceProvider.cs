using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions;
using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

namespace Encamina.Enmarcha.AI;

/// <inheritdoc/>
internal sealed class DefaultCognitiveServiceProvider : CognitiveServiceProviderBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCognitiveServiceProvider"/> class.
    /// </summary>
    /// <param name="conversationAnalysisServiceFactory">A cognitive service factory for conversation analysis cognitive services.</param>
    /// <param name="intentPredictionServiceFactory">A cognitive service factory for intent prediction cognitive services.</param>
    /// <param name="languageDetectionServiceFactory">>A cognitive service factory for language detection cognitive services.</param>
    /// <param name="questionsAnsweringServiceFactory">>A cognitive service factory for questions answering cognitive services.</param>
    /// <param name="textTranslationServiceFactory">>A cognitive service factory for text translation cognitive services.</param>
    public DefaultCognitiveServiceProvider(
        ICognitiveServiceFactory<IConversationAnalysisService> conversationAnalysisServiceFactory = null,
        ICognitiveServiceFactory<IIntentPredictionService> intentPredictionServiceFactory = null,
        ICognitiveServiceFactory<ILanguageDetectionService> languageDetectionServiceFactory = null,
        ICognitiveServiceFactory<IQuestionAnsweringService> questionsAnsweringServiceFactory = null,
        ICognitiveServiceFactory<ITextTranslationService> textTranslationServiceFactory = null)
            : base(conversationAnalysisServiceFactory, intentPredictionServiceFactory, languageDetectionServiceFactory, questionsAnsweringServiceFactory, textTranslationServiceFactory)
    {
    }
}
