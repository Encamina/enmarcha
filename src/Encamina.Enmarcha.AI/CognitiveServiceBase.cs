using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI;

/// <summary>
/// Base class for cognitive services.
/// </summary>
/// <typeparam name="TCognitiveServiceOptions">The configuration service configuration options type.</typeparam>
public class CognitiveServiceBase<TCognitiveServiceOptions> : ICognitiveService
    where TCognitiveServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CognitiveServiceBase{TCognitiveServiceOptions}"/> class.
    /// </summary>
    /// <param name="options">The configuration options for this cognitive service.</param>
    protected CognitiveServiceBase(TCognitiveServiceOptions options)
    {
        Guard.IsNotNull(options);
        Guard.IsNotNull(options.Name);

        Options = options;
    }

    /// <inheritdoc/>
    public string Name => Options.Name;

    /// <summary>
    /// Gets this cognitive service options.
    /// </summary>
    protected virtual TCognitiveServiceOptions Options { get; init; }
}
