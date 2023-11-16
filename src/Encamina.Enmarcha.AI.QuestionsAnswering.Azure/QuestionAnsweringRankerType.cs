namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure;

/// <summary>
/// Represents the type of ranker to be used when sending questions to a knowledge base and determine the best possible answer.
/// </summary>
public enum QuestionAnsweringRankerType
{
    /// <summary>
    /// Use both the question and the answer to determine the best possible answer.
    /// </summary>
    QuestionAndAnswer,

    /// <summary>
    /// Use both the question to determine the best possible answer.
    /// </summary>
    QuestionOnly,
}
