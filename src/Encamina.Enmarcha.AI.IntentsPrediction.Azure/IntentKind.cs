using Encamina.Enmarcha.AI.IntentsPrediction.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// Kind (type) of intent.
/// </summary>
public class IntentKind : IntentKindBase
{
    private const string ConversationAnalysisValue = @"conversation_analysis";
    private const string QuestionAnsweringValue = @"question_answering";

    /// <summary>
    /// Initializes a new instance of the <see cref="IntentKind"/> class.
    /// </summary>
    /// <param name="value">The value of this kind (type) of intent.</param>
    public IntentKind(string value) : base(value)
    {
    }

    /// <summary>
    /// Gets an intent of "Question Answering" kind.
    /// </summary>
    public static IntentKind QuestionAnswering => new(QuestionAnsweringValue);

    /// <summary>
    /// Gets an intent of "Conversation Analysis" kind.
    /// </summary>
    public static IntentKind ConversationAnalysis => new(ConversationAnalysisValue);
}
