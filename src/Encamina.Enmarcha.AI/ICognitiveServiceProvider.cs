using Encamina.Enmarcha.AI.ConversationsAnalysis.Abstractions;
using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;
using Encamina.Enmarcha.AI.LanguagesDetection.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

namespace Encamina.Enmarcha.AI;

/// <summary>
/// Represents a provider for cognitive services.
/// </summary>
public interface ICognitiveServiceProvider
{
    /// <summary>
    /// Retrieves a conversation analysis cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the conversation analysis cognitive service to retrieve.</param>
    /// <returns>
    /// A valid instance of a <see cref="IConversationAnalysisService"/> corresponding to the conversation analysis cognitive service found by its name.
    /// </returns>
    IConversationAnalysisService GetConversationAnalysisService(string serviceName);

    /// <summary>
    /// Retrieves a language detection cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the language detection cognitive service to retrieve.</param>
    /// <returns>
    /// A valid instance of a <see cref="ILanguageDetectionService"/> corresponding to the language detection cognitive service found by its name.
    /// </returns>
    ILanguageDetectionService GetLanguageDetectionService(string serviceName);

    /// <summary>
    /// Retrieves a question answering cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the question answering cognitive service to retrieve.</param>
    /// <returns>
    /// A valid instance of a <see cref="IQuestionAnsweringService"/> corresponding to the question answering cognitive service found by its name.
    /// </returns>
    IQuestionAnsweringService GetQuestionsAnsweringService(string serviceName);

    /// <summary>
    /// Retrieves a text translation cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the text translation cognitive service to retrieve.</param>
    /// <returns>
    /// A valid instance of a <see cref="ITextTranslationService"/> corresponding to the text translation cognitive service found by its name.
    /// </returns>
    ITextTranslationService GetTextTranslationService(string serviceName);

    /// <summary>
    /// Retrieves an intent prediction cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the intent prediction cognitive service to retrieve.</param>
    /// <returns>
    /// A valid instance of a <see cref="IIntentPredictionService"/> corresponding to the intent prediction cognitive service found by its name.
    /// </returns>
    IIntentPredictionService GetIntentPredictionService(string serviceName);

    /// <summary>
    /// Tries to retrieve a conversation analysis cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the conversation analysis cognitive service to retrieve.</param>
    /// <param name="service">When this method returns, the value associated with the specified <paramref name="serviceName"/>,
    /// if the service is found; otherwise, <see langword="null"/>. <b>This parameter is passed uninitialized</b>.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a service is found; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetConversationAnalysisService(string serviceName, out IConversationAnalysisService? service);

    /// <summary>
    /// Tries to retrieve  a language detection cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the language detection cognitive service to retrieve.</param>
    /// <param name="service">When this method returns, the value associated with the specified <paramref name="serviceName"/>,
    /// if the service is found; otherwise, <see langword="null"/>. <b>This parameter is passed uninitialized</b>.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a service is found; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetLanguageDetectionService(string serviceName, out ILanguageDetectionService? service);

    /// <summary>
    /// Tries to retrieve  a question answering cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the question answering cognitive service to retrieve.</param>
    /// <param name="service"> When this method returns, the value associated with the specified <paramref name="serviceName"/>,
    /// if the service is found; otherwise, <see langword="null"/>. <b>This parameter is passed uninitialized</b>.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a service is found; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetQuestionsAnsweringService(string serviceName, out IQuestionAnsweringService? service);

    /// <summary>
    /// Tries to retrieve  a text translation cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the text translation cognitive service to retrieve.</param>
    /// <param name="service">When this method returns, the value associated with the specified <paramref name="serviceName"/>,
    /// if the service is found; otherwise, <see langword="null"/>. <b>This parameter is passed uninitialized</b>.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a service is found; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetTextTranslationService(string serviceName, out ITextTranslationService? service);

    /// <summary>
    /// Tries to retrieve  an intent prediction cognitive service given its name.
    /// </summary>
    /// <param name="serviceName">The name of the intent prediction cognitive service to retrieve.</param>
    /// <param name="service">When this method returns, the value associated with the specified <paramref name="serviceName"/>,
    /// if the service is found; otherwise, <see langword="null"/>. <b>This parameter is passed uninitialized</b>.
    /// </param>
    /// <returns>
    /// Returns <see langword="true"/> if a service is found; otherwise, returns <see langword="false"/>.
    /// </returns>
    bool TryGetIntentPredictionService(string serviceName, out IIntentPredictionService? service);
}
