namespace Encamina.Enmarcha.AI.Abstractions;

/// <summary>
/// Base clase for cognitive services configuration of its options.
/// </summary>
/// <typeparam name="TCognitiveServiceOptions">
/// The type of cognitive service options for this configuration.
/// </typeparam>
public interface ICognitiveServiceConfigurationsBase<out TCognitiveServiceOptions> where TCognitiveServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Gets the collection of common cognitive service options in this configuration.
    /// </summary>
    public IReadOnlyList<TCognitiveServiceOptions> CognitiveServiceOptions { get; }
}
