using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure;

/// <summary>
/// Options to configure the question answering service from Azure.
/// </summary>
internal record QuestionAnsweringServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Gets minimum confidence threshold score for answers, value ranges from 0 to 1.
    /// </summary>
    public double? ConfidenceThreshold { get; init; }

    /// <summary>
    /// Gets the deployment slot of the knowledge base to use.
    /// Valid values are <c>test</c> and <c>prod</c> (or <c>production</c> which is also valid).
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string DeploymentSlot { get; init; }

    /// <summary>
    /// Gets the name of the knowledge base.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string KnowledgeBaseName { get; init; }

    /// <summary>
    /// Gets the default language to use in some client methods. This is the <see href="https://en.wikipedia.org/wiki/IETF_language_tag">BCP-47</see>
    /// representation of a language. For example, use "en" for English, "es" for Spanish, etc. If not set, the service default ("en" for English) is used.
    /// </summary>
    public string KnowledgeBaseLanguage { get; init; } = @"en";

    /// <summary>
    /// Gets the maximum number of answers to be returned for the question.
    /// </summary>
    public int? Top { get; init; }

    /// <summary>
    /// Gets the type of ranker to be used when sending questions to a knowledge base and determine the best possible answer.
    /// Defailts to '<see cref="QuestionAnsweringRankerType.QuestionAndAnswer"/>'.
    /// </summary>
    public QuestionAnsweringRankerType QuestionAnsweringRankerType { get; init; } = QuestionAnsweringRankerType.QuestionAndAnswer;
}
