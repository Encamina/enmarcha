using System.Text.Json;

using AdaptiveCards.Templating;

using Encamina.Enmarcha.Agents.Models;
using Encamina.Enmarcha.Agents.Options;

using Encamina.Enmarcha.Conversation.Abstractions;

using Microsoft.Agents.Builder;
using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Extensions.Teams;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.Agents.Activities;

/// <summary>
/// Manages live activities by handling updates, maintaining history, generating adaptive cards, and sending/updating activities.
/// </summary>
public sealed class LiveActivityManager
{
    private readonly IIntentResponsesProvider intentResponsesProvider;

    private readonly IDistributedCache cache;

    private readonly ILogger<LiveActivityManager> logger;

    private readonly JsonSerializerOptions jsonSerializerOptions;

    private LiveActivityManagerOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveActivityManager"/> class.
    /// </summary>
    /// <param name="intentResponsesProvider">Provider for localized intent responses.</param>
    /// <param name="cache">Distributed cache for storing live activity entries.</param>
    /// <param name="optionsMonitor">Options monitor for <see cref="LiveActivityManagerOptions"/>.</param>
    /// <param name="logger">A logger for this class.</param>
    public LiveActivityManager(IIntentResponsesProvider intentResponsesProvider, IDistributedCache cache, IOptionsMonitor<LiveActivityManagerOptions> optionsMonitor, ILogger<LiveActivityManager> logger)
    {
        this.intentResponsesProvider = intentResponsesProvider;
        this.cache = cache;
        this.logger = logger;

        options = optionsMonitor.CurrentValue;
        optionsMonitor.OnChange(o => options = o);

        jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        };
    }

    /// <summary>
    /// Handles an update to a live activity, updating its history, generating an adaptive card, and sending/updating the activity.
    /// </summary>
    /// <param name="turnContext">The turn context for the current bot activity.</param>
    /// <param name="req">The live activity update request containing the update details.</param>
    /// <param name="channelId">The channel ID (e.g., "msteams", "ems", "directline", "webchat").</param>
    /// <param name="ct">A cancellation token for the async operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task HandleUpdateAsync(ITurnContext turnContext, LiveActivityUpdateRequest req, string channelId, CancellationToken ct)
    {
        var channelIsCopilot = channelId.Equals(Channels.M365Copilot);

        var entry = await GetEntryAsync(req.LiveActivityId, ct) ?? new LiveActivityEntry { LiveActivityId = req.LiveActivityId };

        // Limited history
        entry.History.Add(req);
        if (entry.History.Count > options.HistoryLimit)
        {
            var toRemove = entry.History.Count - options.HistoryLimit;
            entry.History.RemoveRange(0, toRemove);
        }

        await SendNowAsync(turnContext, entry, channelIsCopilot, ct);

        await SaveEntryAsync(entry, ct);
    }

    private static string CacheKey(string liveActivityId) => $"liveactivity:{liveActivityId}";

    private static string? FixNewLines(string? text) => text?.Replace(@"\\n", Environment.NewLine);

    private async Task SendNowAsync(ITurnContext turnContext, LiveActivityEntry entry, bool channelIsCopilot, CancellationToken ct)
    {
        // Copilot: if not finished, don't update (it will be updated when finished)
        if (channelIsCopilot && entry.History.Last().Status == LiveActivityStatus.Running)
        {
            return;
        }

        // Copilot finished, or normal Teams: create or update card
        var attachment = await BuildCardAttachment(entry.History, turnContext.Activity.Locale, channelIsCopilot, ct);

        // Create activity if needed. (Cause we don't persist ActivityId for Copilot, it will be always null there)
        if (string.IsNullOrEmpty(entry.ActivityId))
        {
            var message = MessageFactory.Attachment(attachment);
            message.TeamsNotifyUser();
            var rr = await turnContext.SendActivityAsync(message, ct);
            entry.ActivityId = rr.Id;

            return;
        }

        // Update activity otherwise
        var update = MessageFactory.Attachment(attachment);
        var act = (Activity)update;
        act.Id = entry.ActivityId;

        await turnContext.UpdateActivityAsync(act, ct);
    }

    private async Task<LiveActivityEntry?> GetEntryAsync(string liveActivityId, CancellationToken ct)
    {
        var json = await cache.GetStringAsync(CacheKey(liveActivityId), ct);
        return string.IsNullOrEmpty(json) ? null : JsonSerializer.Deserialize<LiveActivityEntry>(json, jsonSerializerOptions);
    }

    private Task SaveEntryAsync(LiveActivityEntry entry, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(entry);
        var opts = new DistributedCacheEntryOptions { SlidingExpiration = options.CacheSlidingExpiration };
        return cache.SetStringAsync(CacheKey(entry.LiveActivityId), json, opts, ct);
    }

    private async Task<Attachment> BuildCardAttachment(IReadOnlyList<LiveActivityUpdateRequest> history, string locale, bool channelIsCopilot, CancellationToken ct)
    {
        if (history == null || history.Count == 0)
        {
            var jsonEmpty = new AdaptiveCardTemplate(options.LiveTemplateJson).Expand(new
            {
                title = string.Empty,
                subtitle = (string?)null,
                content = (string?)null,
                statusText = "Unknown",
                statusColor = "default",
                statusGlyph = "❔",
                progressPercentText = (string?)null,
                showProgress = false,
                showStatusBar = !channelIsCopilot,
                isRunning = false,
                hasHistory = false,
                history = Array.Empty<object>(),
            });

            return new Attachment
            {
                ContentType = Encamina.Enmarcha.Net.Http.MediaTypeNames.Application.AdaptativeCard,
                Content = JsonSerializer.Deserialize<object>(jsonEmpty, jsonSerializerOptions),
            };
        }

        // Current
        var current = history[^1];
        var (currText, currColor, currGlyph) = await MapStatus(current.Status, locale, ct);
        var currHasProgress = current.ProgressPercent.HasValue;
        var currPercent = Math.Clamp(current.ProgressPercent ?? 0, 0, 100);

        // Past
        var past = new List<object>(Math.Max(0, history.Count - 1));
        for (var i = 0; i <= history.Count - 2; i++)
        {
            var it = history[i];
            past.Add(new
            {
                stepText = $"{i + 1}.",
                title = FixNewLines(it.Title),
                content = FixNewLines(it.Content),
                subtitle = FixNewLines(it.Subtitle),
            });
        }

        var json = new AdaptiveCardTemplate(options.LiveTemplateJson).Expand(new
        {
            // Current
            title = FixNewLines(current.Title),
            subtitle = FixNewLines(current.Subtitle),
            content = FixNewLines(current.Content),
            statusText = currText,
            statusColor = currColor,
            statusGlyph = currGlyph,
            progressPercentText = currHasProgress ? $"{currPercent}%" : null,
            showProgress = currHasProgress,
            showStatusBar = !channelIsCopilot,
            isRunning = current.Status == LiveActivityStatus.Running,

            // History
            hasHistory = past.Count > 0 && current.ShowHistory,
            history = past,
        });

        return new Attachment
        {
            ContentType = Encamina.Enmarcha.Net.Http.MediaTypeNames.Application.AdaptativeCard,
            Content = JsonSerializer.Deserialize<object>(json, jsonSerializerOptions),
        };
    }

    private async Task<(string Text, string Color, string Glyph)> MapStatus(LiveActivityStatus s, string locale, CancellationToken ct)
    {
        var responses = await intentResponsesProvider.GetResponsesAsync(s.ToString(), locale, ct);
        var translatedText = responses.FirstOrDefault()?.Text;

        if (string.IsNullOrEmpty(translatedText))
        {
            logger.LogWarning("No translation found for status {Status} and locale {Locale}. Falling back to enum name.", s, locale);
            translatedText = s.ToString();
        }

        return s switch
        {
            LiveActivityStatus.Running => (translatedText, "accent", "⏳"),
            LiveActivityStatus.Completed => (translatedText, "good", "✅"),
            LiveActivityStatus.Warning => (translatedText, "warning", "⚠️"),
            LiveActivityStatus.Failed => (translatedText, "attention", "❌"),
            _ => (translatedText, "default", "❔"),
        };
    }

    private sealed class LiveActivityEntry
    {
        public required string LiveActivityId { get; init; }

        public string? ActivityId { get; set; }

        public List<LiveActivityUpdateRequest> History { get; init; } = [];
    }
}
