namespace Encamina.Enmarcha.SemanticKernel.Options;

/// <summary>
/// Configuration options for an «Ephemeral Memory Store Handler».
/// </summary>
public sealed class EphemeralMemoryStoreHandlerOptions
{
    /// <summary>
    /// Gets the idle time in minutes after which a memory is considered inactive and can be removed from the memory store.
    /// </summary>
    /// <remarks>Defaults to 60 minutes.</remarks>
    public int IdleTimeoutMinutes { get; init; } = 60;

    /// <summary>
    /// Gets the time in minutes to wait before polling for inactive memories.
    /// </summary>
    /// <remarks>Defaults to 10 minutes.</remarks>
    public int InactivePollingTimeMinutes { get; init; } = 10;
}
