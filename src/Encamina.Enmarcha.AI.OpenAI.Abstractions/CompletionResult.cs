using Encamina.Enmarcha.Entities.Abstractions;

namespace Encamina.Enmarcha.AI.OpenAI.Abstractions;

/// <summary>
/// Response from completions request for OpenAI.
/// </summary>
public class CompletionResult : IdentifiableBase<string>
{
    /// <summary>
    /// Gets the date and time in UTC when the response has been receeived.
    /// </summary>
    public DateTime CreatedUtc { get; init; }

    /// <summary>
    /// Gets the completions generated.
    /// </summary>
    public IEnumerable<Completition> Completitions { get; init; }
}
