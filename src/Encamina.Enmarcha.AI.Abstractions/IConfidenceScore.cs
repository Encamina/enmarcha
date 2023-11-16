namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Represents the confidence score.
/// </summary>
public interface IConfidenceScore
{
    /// <summary>
    /// Gets value that represents the confidence score.
    /// </summary>
    double? ConfidenceScore { get; }
}
