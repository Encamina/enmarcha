using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

using Encamina.Enmarcha.Bot.Abstractions.QuestionAnswering;
using Encamina.Enmarcha.Conversation.Abstractions;

using Microsoft.Bot.Builder;

namespace Encamina.Enmarcha.Bot.Skills.QuestionAnswering;

internal record class QuestionAnsweringDialogServices(ICognitiveServiceFactory<IQuestionAnsweringService> QuestionAnsweringCognitiveServiceFactory,
    IBotTelemetryClient? BotTelemetryClient = null,
    ISourcesProcessor? SourcesProcessor = null,
    IMetadataProcessor? MetadataProcessor = null,
    IQuestionRequestProcessor? QuestionRequestProcessor = null,
    IQuestionResultProcessor? QuestionResultProcessor = null,
    ISendAnswersProcessor? SendAnswersProcessor = null,
    IIntentResponsesProvider? IntentResponsesProvider = null);
