using Encamina.Enmarcha.Agents.Abstractions.QuestionAnswering;
using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;
using Encamina.Enmarcha.Conversation.Abstractions;

namespace Encamina.Enmarcha.Agents.Skills.QuestionAnswering;

/// <summary>
/// Represents a collection of services and processors used to facilitate question-answering dialog functionality.
/// </summary>
/// <param name="QuestionAnsweringCognitiveServiceFactory">A factory for creating instances of <see cref="IQuestionAnsweringService"/> to handle question-answering operations.
/// This parameter is required.</param>
/// <param name="SourcesProcessor">An optional processor for handling source-related operations, such as filtering or transforming input sources.</param>
/// <param name="MetadataProcessor">An optional processor for managing metadata associated with question-answering requests or responses.</param>
/// <param name="QuestionRequestProcessor">An optional processor for pre-processing question requests before they are sent to the question-answering service.</param>
/// <param name="QuestionResultProcessor">An optional processor for post-processing results returned by the question-answering service.</param>
/// <param name="SendAnswersProcessor">An optional processor for managing the delivery of answers to the user or downstream systems.</param>
/// <param name="IntentResponsesProvider">An optional provider for retrieving intent-specific responses, which can be used to customize the dialog experience.</param>
internal record QuestionAnsweringDialogServices(ICognitiveServiceFactory<IQuestionAnsweringService> QuestionAnsweringCognitiveServiceFactory,
    ISourcesProcessor? SourcesProcessor = null,
    IMetadataProcessor? MetadataProcessor = null,
    IQuestionRequestProcessor? QuestionRequestProcessor = null,
    IQuestionResultProcessor? QuestionResultProcessor = null,
    ISendAnswersProcessor? SendAnswersProcessor = null,
    IIntentResponsesProvider? IntentResponsesProvider = null);
