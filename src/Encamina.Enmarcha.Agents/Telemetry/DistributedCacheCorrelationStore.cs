using Encamina.Enmarcha.Agents.Abstractions.Telemetry;

using Microsoft.Extensions.Caching.Distributed;

namespace Encamina.Enmarcha.Agents.Telemetry;

/// <summary>
/// An implementation of <see cref="ICorrelationStore"/> using <see cref="IDistributedCache"/>.
/// </summary>
public sealed class DistributedCacheCorrelationStore : ICorrelationStore
{
    private readonly IDistributedCache cache;
    private readonly string prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCacheCorrelationStore"/> class.
    /// </summary>
    /// <param name="cache">A distributed cache instance.</param>
    /// <param name="keyPrefix">A prefix to use for cache keys.</param>
    public DistributedCacheCorrelationStore(IDistributedCache cache, string keyPrefix = "corr:")
    {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        prefix = keyPrefix;
    }

    /// <inheritdoc/>
    public async ValueTask SetAsync(string conversationId, string activityId, CorrelationEntry entry, TimeSpan ttl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || string.IsNullOrWhiteSpace(activityId))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(entry.TraceParent))
        {
            return;
        }

        var key = Key(conversationId, activityId);
        var value = Pack(entry.TraceParent, entry.TraceState);

        var opts = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
        };

        await cache.SetStringAsync(key, value, opts, ct).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<CorrelationEntry?> GetAsync(string conversationId, string activityId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || string.IsNullOrWhiteSpace(activityId))
        {
            return null;
        }

        var key = Key(conversationId, activityId);
        var packed = await cache.GetStringAsync(key, ct).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(packed))
        {
            return null;
        }

        var (tp, ts) = Unpack(packed);
        return new CorrelationEntry(tp, ts);
    }

    private static string Pack(string tp, string? ts) => string.IsNullOrWhiteSpace(ts) ? tp : $"{tp}|{ts}";

    private static (string Tp, string? Ts) Unpack(string s)
    {
        var i = s.IndexOf('|');
        return i < 0 ? (s, null) : (s[..i], s[(i + 1)..]);
    }

    private string Key(string conv, string act) => $"{prefix}{conv}:{act}";
}
