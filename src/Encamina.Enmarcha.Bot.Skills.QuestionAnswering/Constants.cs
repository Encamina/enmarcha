using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Skills.QuestionAnswering;

/// <summary>
/// Constants related to the question answering skill.
/// </summary>
internal static class Constants
{
    /// <summary>
    /// Default intent for the question answering dialog that can be obteined from <see cref="QuestionAnsweringDialog.Intent"/>.
    /// </summary>
    internal const string DefaultIntent = @"QuestionAnswering";

    /// <summary>
    /// Intent for the question answering dialog to respond when no suitable answer could be found.
    /// </summary>
    internal const string ConfusedIntent = @"Confused";

    /// <summary>
    /// Prefix to use for trace activities for answers.
    /// </summary>
    internal const string AnswerTraceActivityNamePrefix = @"AnswerTrace-";

    /// <summary>
    /// Label for a parameter that can be send (usually as a JSON payload) in an <see cref="Activity.Value"/> to request verbose tracing from this skill.
    /// </summary>
    internal const string ActivityValueVerbose = @"x-encamina-enmarcha-bot-skill-questionanswering-verbose";
}
