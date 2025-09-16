using System.Text.Json;
using System.Text.Json.Serialization;

using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Agents.Abstractions.Telemetry;
using Encamina.Enmarcha.Agents.Telemetry;

using Microsoft.Agents.Builder;

using Microsoft.Agents.Core.Models;
using Microsoft.Agents.Extensions.Teams.Models;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// Uses a <see cref="IAgentTelemetryClient"/> object to log incoming, outgoing, updated, or deleted message activities.
/// </summary>
public class TelemetryLoggerMiddleware : IMiddleware
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { MaxDepth = int.MaxValue };

    /// <summary>
    /// Initializes a new instance of the <see cref="TelemetryLoggerMiddleware"/> class.
    /// </summary>
    /// <param name="telemetryClient">The telemetry client to send telemetry events to.</param>
    /// <param name="logPersonalInformation">`true` to include personally identifiable information; otherwise, `false`.</param>
    public TelemetryLoggerMiddleware(IAgentTelemetryClient? telemetryClient, bool logPersonalInformation = false)
    {
        TelemetryClient = telemetryClient ?? new NullAgentTelemetryClient();
        LogPersonalInformation = logPersonalInformation;
    }

    /// <summary>
    /// Gets a value indicating whether to include personal information that came from the user.
    /// </summary>
    /// <value>`true` to include personally identifiable information; otherwise, `false`.</value>
    /// <remarks>
    /// If true, personal information is included in calls to the telemetry client's
    /// <see cref="IAgentTelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/> method;
    /// otherwise this information is filtered out.
    /// </remarks>
    public bool LogPersonalInformation { get; }

    /// <summary>
    /// Gets The telemetry client to send telemetry events to.
    /// </summary>
    /// <value>
    /// The <see cref="IAgentTelemetryClient"/> this middleware uses to log events.
    /// </value>
    [JsonIgnore]
    public IAgentTelemetryClient TelemetryClient { get; }

    /// <summary>
    /// Logs events for incoming, outgoing, updated, or deleted message activities, using the <see cref="TelemetryClient"/>.
    /// </summary>
    /// <param name="context">The context object for this turn.</param>
    /// <param name="nextTurn">The delegate to call to continue the agent middleware pipeline.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    /// <seealso cref="ITurnContext"/>
    /// <seealso cref="IActivity"/>
    public virtual async Task OnTurnAsync(ITurnContext context, NextDelegate nextTurn, CancellationToken cancellationToken)
    {
        Guard.IsNotNull(context);

        // log incoming activity at beginning of turn
        if (context.Activity != null)
        {
            var activity = context.Activity;

            // Log Agent Message Received
            await OnReceiveActivityAsync(activity, cancellationToken).ConfigureAwait(false);
        }

        // hook up onSend pipeline
        context.OnSendActivities(async (_, activities, nextSend) =>
        {
            // run full pipeline
            var responses = await nextSend().ConfigureAwait(false);

            foreach (var activity in activities)
            {
                await OnSendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
            }

            return responses;
        });

        // hook up update activity pipeline
        context.OnUpdateActivity(async (_, activity, nextUpdate) =>
        {
            // run full pipeline
            var response = await nextUpdate().ConfigureAwait(false);

            await OnUpdateActivityAsync(activity, cancellationToken).ConfigureAwait(false);

            return response;
        });

        // hook up delete activity pipeline
        context.OnDeleteActivity(async (_, reference, nextDelete) =>
        {
            // run full pipeline
            await nextDelete().ConfigureAwait(false);

            var deleteActivity = new Activity
            {
                Type = ActivityTypes.MessageDelete,
                Id = reference.ActivityId,
            }
            .ApplyConversationReference(reference, isIncoming: false) as IMessageDeleteActivity;

            await OnDeleteActivityAsync(deleteActivity, cancellationToken).ConfigureAwait(false);
        });

        if (nextTurn != null)
        {
            await nextTurn(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Uses the telemetry client's
    /// <see cref="IAgentTelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/> method to
    /// log telemetry data when a message is received from the user.
    /// The event name is <see cref="TelemetryLoggerConstants.AgentMsgReceiveEvent"/>.
    /// </summary>
    /// <param name="activity">Current activity sent from user.</param>
    /// <param name="cancellation">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    protected virtual async Task OnReceiveActivityAsync(IActivity activity, CancellationToken cancellation)
    {
        TelemetryClient.TrackEvent(TelemetryLoggerConstants.AgentMsgReceiveEvent, await FillReceiveEventPropertiesAsync(activity).ConfigureAwait(false));
    }

    /// <summary>
    /// Uses the telemetry client's
    /// <see cref="IAgentTelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/> method to
    /// log telemetry data when the agent sends the user a message. It uses the telemetry client's
    /// The event name is <see cref="TelemetryLoggerConstants.AgentMsgSendEvent"/>.
    /// </summary>
    /// <param name="activity">Current activity sent from user.</param>
    /// <param name="cancellation">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    protected virtual async Task OnSendActivityAsync(IActivity activity, CancellationToken cancellation)
    {
        TelemetryClient.TrackEvent(TelemetryLoggerConstants.AgentMsgSendEvent, await FillSendEventPropertiesAsync(activity).ConfigureAwait(false));
    }

    /// <summary>
    /// Uses the telemetry client's
    /// <see cref="IAgentTelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/> method to
    /// log telemetry data when the agent updates a message it sent previously.
    /// The event name is <see cref="TelemetryLoggerConstants.AgentMsgUpdateEvent"/>.
    /// </summary>
    /// <param name="activity">Current activity sent from user.</param>
    /// <param name="cancellation">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    protected virtual async Task OnUpdateActivityAsync(IActivity activity, CancellationToken cancellation)
    {
        TelemetryClient.TrackEvent(TelemetryLoggerConstants.AgentMsgUpdateEvent, await FillUpdateEventPropertiesAsync(activity).ConfigureAwait(false));
    }

    /// <summary>
    /// Uses the telemetry client's
    /// <see cref="IAgentTelemetryClient.TrackEvent(string, IDictionary{string, string}, IDictionary{string, double})"/> method to
    /// log telemetry data when the agent deletes a message it sent previously.
    /// The event name is <see cref="TelemetryLoggerConstants.AgentMsgDeleteEvent"/>.
    /// </summary>
    /// <param name="activity">Current activity sent from user.</param>
    /// <param name="cancellation">A cancellation token that can be used by other objects
    /// or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the work queued to execute.</returns>
    protected virtual async Task OnDeleteActivityAsync(IMessageDeleteActivity? activity, CancellationToken cancellation)
    {
        TelemetryClient.TrackEvent(TelemetryLoggerConstants.AgentMsgDeleteEvent, await FillDeleteEventPropertiesAsync(activity).ConfigureAwait(false));
    }

    /// <summary>
    /// Fills event properties for the <see cref="TelemetryLoggerConstants.AgentMsgReceiveEvent"/> event.
    /// If the <see cref="LogPersonalInformation"/> is true, filters out the sender's name and the
    /// message's text and speak fields.
    /// </summary>
    /// <param name="activity">The message activity sent from user.</param>
    /// <param name="additionalProperties">Additional properties to add to the event.</param>
    /// <returns>The properties and their values to log when a message is received from the user.</returns>
    protected Task<Dictionary<string, string>> FillReceiveEventPropertiesAsync(IActivity activity, Dictionary<string, string>? additionalProperties = null)
    {
        if (activity == null)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        var properties = new Dictionary<string, string>()
        {
            { TelemetryConstants.FromIdProperty, activity.From?.Id ?? string.Empty },
            { TelemetryConstants.ConversationNameProperty, activity.Conversation?.Name ?? string.Empty },
            { TelemetryConstants.LocaleProperty, activity.Locale },
            { TelemetryConstants.RecipientIdProperty, activity.Recipient?.Id ?? string.Empty },
            { TelemetryConstants.RecipientNameProperty, activity.Recipient?.Name ?? string.Empty },
            { TelemetryConstants.ActivityTypeProperty, activity.Type },
            { TelemetryConstants.ConversationIdProperty, activity.Conversation?.Id ?? string.Empty },
            { TelemetryConstants.ActivityIdProperty, activity.Id },
        };

        // Use the LogPersonalInformation flag to toggle logging PII data, text and username are common examples
        if (LogPersonalInformation)
        {
            if (!string.IsNullOrWhiteSpace(activity.From?.Name))
            {
                properties.Add(TelemetryConstants.FromNameProperty, activity.From.Name);
            }

            if (!string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(TelemetryConstants.TextProperty, activity.Text);
            }

            if (!string.IsNullOrWhiteSpace(activity.Speak))
            {
                properties.Add(TelemetryConstants.SpeakProperty, activity.Speak);
            }
        }

        PopulateAdditionalChannelProperties(activity, properties);

        // Additional Properties can override "stock" properties.
        if (additionalProperties != null)
        {
            return Task.FromResult(additionalProperties.Concat(properties)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.First().Value));
        }

        return Task.FromResult(properties);
    }

    /// <summary>
    /// Fills event properties for the <see cref="TelemetryLoggerConstants.AgentMsgSendEvent"/> event.
    /// If the <see cref="LogPersonalInformation"/> is true, filters out the recipient's name and the
    /// message's text and speak fields.
    /// </summary>
    /// <param name="activity">The user's activity to which the agent is responding.</param>
    /// <param name="additionalProperties">Additional properties to add to the event.</param>
    /// <returns>The properties and their values to log when the agent sends the user a message.</returns>
    protected Task<Dictionary<string, string>> FillSendEventPropertiesAsync(IActivity activity, Dictionary<string, string>? additionalProperties = null)
    {
        if (activity == null)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        var properties = new Dictionary<string, string>()
        {
            { TelemetryConstants.ReplyActivityIDProperty, activity.ReplyToId },
            { TelemetryConstants.RecipientIdProperty, activity.Recipient?.Id ?? string.Empty },
            { TelemetryConstants.ConversationNameProperty, activity.Conversation?.Name ?? string.Empty },
            { TelemetryConstants.LocaleProperty, activity.Locale },
            { TelemetryConstants.ActivityTypeProperty, activity.Type },
            { TelemetryConstants.ConversationIdProperty, activity.Conversation?.Id ?? string.Empty },
            { TelemetryConstants.ActivityIdProperty, activity.Id },
        };

        // Use the LogPersonalInformation flag to toggle logging PII data, text and user name are common examples
        if (LogPersonalInformation)
        {
            if (!string.IsNullOrWhiteSpace(activity.Recipient?.Name))
            {
                properties.Add(TelemetryConstants.RecipientNameProperty, activity.Recipient.Name);
            }

            if (!string.IsNullOrWhiteSpace(activity.Text))
            {
                properties.Add(TelemetryConstants.TextProperty, activity.Text);
            }

            if (!string.IsNullOrWhiteSpace(activity.Speak))
            {
                properties.Add(TelemetryConstants.SpeakProperty, activity.Speak);
            }

            if (activity.Attachments != null && activity.Attachments.Any())
            {
                properties.Add(TelemetryConstants.AttachmentsProperty, JsonSerializer.Serialize(activity.Attachments, JsonSerializerOptions));
            }
        }

        // Additional Properties can override "stock" properties.
        if (additionalProperties != null)
        {
            return Task.FromResult(additionalProperties.Concat(properties)
                       .GroupBy(kv => kv.Key)
                       .ToDictionary(g => g.Key, g => g.First().Value));
        }

        return Task.FromResult(properties);
    }

    /// <summary>
    /// Fills event properties for the <see cref="TelemetryLoggerConstants.AgentMsgUpdateEvent"/> event.
    /// If the <see cref="LogPersonalInformation"/> is true, filters out the message's text field.
    /// </summary>
    /// <param name="activity">Last activity sent from user.</param>
    /// <param name="additionalProperties">Additional properties to add to the event.</param>
    /// <returns>The properties and their values to log when the agent updates a message it sent previously.</returns>
    protected Task<Dictionary<string, string>> FillUpdateEventPropertiesAsync(IActivity activity, Dictionary<string, string>? additionalProperties = null)
    {
        if (activity == null)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        var properties = new Dictionary<string, string>()
        {
            { TelemetryConstants.RecipientIdProperty, activity.Recipient?.Id ?? string.Empty },
            { TelemetryConstants.ConversationIdProperty, activity.Conversation?.Id ?? string.Empty },
            { TelemetryConstants.ConversationNameProperty, activity.Conversation?.Name ?? string.Empty },
            { TelemetryConstants.LocaleProperty, activity.Locale },
            { TelemetryConstants.ActivityTypeProperty, activity.Type },
            { TelemetryConstants.ActivityIdProperty, activity.Id },
        };

        // Use the LogPersonalInformation flag to toggle logging PII data, text is a common example
        if (LogPersonalInformation && !string.IsNullOrWhiteSpace(activity.Text))
        {
            properties.Add(TelemetryConstants.TextProperty, activity.Text);
        }

        // Additional Properties can override "stock" properties.
        if (additionalProperties != null)
        {
            return Task.FromResult(additionalProperties.Concat(properties)
                       .GroupBy(kv => kv.Key)
                       .ToDictionary(g => g.Key, g => g.First().Value));
        }

        return Task.FromResult(properties);
    }

    /// <summary>
    /// Fills event properties for the <see cref="TelemetryLoggerConstants.AgentMsgDeleteEvent"/> event.
    /// </summary>
    /// <param name="activity">The Activity object deleted by agent.</param>
    /// <param name="additionalProperties">Additional properties to add to the event.</param>
    /// <returns>The properties and their values to log when the agent deletes a message it sent previously.</returns>
#pragma warning disable CA1822 // Mark members as static (can't change this without breaking binary compat)
    protected Task<Dictionary<string, string>> FillDeleteEventPropertiesAsync(IMessageDeleteActivity? activity, Dictionary<string, string>? additionalProperties = null)
#pragma warning restore CA1822 // Mark members as static
    {
        if (activity == null)
        {
            return Task.FromResult(new Dictionary<string, string>());
        }

        var properties = new Dictionary<string, string>()
        {
            { TelemetryConstants.RecipientIdProperty, activity.Recipient?.Id ?? string.Empty },
            { TelemetryConstants.ConversationIdProperty, activity.Conversation?.Id ?? string.Empty },
            { TelemetryConstants.ConversationNameProperty, activity.Conversation?.Name ?? string.Empty },
            { TelemetryConstants.ActivityTypeProperty, activity.Type },
            { TelemetryConstants.ActivityIdProperty, activity.Id },
        };

        // Additional Properties can override "stock" properties.
        if (additionalProperties != null)
        {
            return Task.FromResult(additionalProperties.Concat(properties)
                       .GroupBy(kv => kv.Key)
                       .ToDictionary(g => g.Key, g => g.First().Value));
        }

        return Task.FromResult(properties);
    }

    private static void PopulateAdditionalChannelProperties(IActivity activity, Dictionary<string, string> properties)
    {
        switch (activity.ChannelId)
        {
            case Channels.Msteams:
                var teamsChannelData = activity.GetChannelData<TeamsChannelData>();

                properties.Add("TeamsTenantId", teamsChannelData?.Tenant?.Id ?? string.Empty);
                properties.Add("TeamsUserAadObjectId", activity.From?.AadObjectId ?? string.Empty);

                if (teamsChannelData?.Team != null)
                {
                    properties.Add("TeamsTeamInfo", JsonSerializer.Serialize(teamsChannelData.Team, JsonSerializerOptions));
                }

                break;
        }
    }
}