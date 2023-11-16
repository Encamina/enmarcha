using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// Options to configure the intent prediction (actually conversation analysis) service from Azure.
/// </summary>
internal record IntentPredictionServiceOptions : CognitiveServiceOptionsBase
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
    /// Gets a value indicating whether the prediction should be kept for further review
    /// to improve the model quality. Default is <see langword="false"/>.
    /// </summary>
    public bool EnableLogging { get; init; } = false;

    /// <summary>
    /// Gets a value indicating whether the prediction should include more detailed information
    /// in its result. Default is <see langword="false"/>.
    /// </summary>
    public bool EnableVerbose { get; init; } = false;

    /// <summary>
    /// Gets the name of the knowledge base.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string OrchestratorName { get; init; }
}
