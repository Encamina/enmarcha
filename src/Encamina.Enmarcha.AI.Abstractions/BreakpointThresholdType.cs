namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Type of thresholds used for breakpoints in <see cref="ISemanticTextSplitter"/>.
/// </summary>
public enum BreakpointThresholdType
{
    /// <summary>
    /// Threshold based on percentiles for breakpoints.
    /// </summary>
    Percentile,

    /// <summary>
    /// Threshold based on standard deviations for breakpoints.
    /// </summary>
    StandardDeviation,

    /// <summary>
    /// Threshold based on interquartile range for breakpoints.
    /// </summary>
    Interquartile,
}
