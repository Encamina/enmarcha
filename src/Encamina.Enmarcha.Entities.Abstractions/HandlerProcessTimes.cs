namespace Encamina.Enmarcha.Entities.Abstractions;

/// <summary>
/// Represents the process time when a specific handler should be executed.
/// </summary>
/// <remarks>
/// To avoid spelling out the decimal values of powers of two, the left-shift operator is used to declare this enum's values.
/// </remarks>
[Flags]
public enum HandlerProcessTimes
{
    /// <summary>
    /// Indicates the handled should never be processed.
    /// </summary>
    None = 0,

    /// <summary>
    /// Indicates the handler should be processed at the beginning of the turn context.
    /// </summary>
    Begin = 1 << 0,

    /// <summary>
    /// Indicates the handler should be processed at the ending of the turn context.
    /// </summary>
    End = 1 << 1,

    /// <summary>
    /// Indicates the handler should be processed at both, the beginning and the ending of the turn context.
    /// </summary>
    Both = Begin | End,
}
