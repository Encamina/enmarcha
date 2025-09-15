using System.Globalization;

using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Abstractions.Extensions;

/// <summary>
/// Extension helper methods for <see cref="Activity"/>.
/// </summary>
public static class ActivityExtensions
{
    private const string Add = @"add";

    /// <summary>
    /// Determines if the activity corresponds to a start of a conversation.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the activity is a start activity, otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool IsStartActivity(this IActivity activity)
    {
        return activity.ChannelId.ToString() switch
        {
            Channels.Skype => activity is { Type: ActivityTypes.ContactRelationUpdate, Action: Add },
            Channels.Directline or
            Channels.Emulator or
            Channels.Webchat or
            Channels.Msteams
                => activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id),
            _ => false,
        };
    }

    /// <summary>
    /// Gets a <see cref="CultureInfo"/> from an activity's <see cref="Activity.Locale"/> property. If, for any reason, the
    /// property is <see langword="null"/> or empty, then the culture from <see cref="CultureInfo.CurrentCulture"/> is
    /// returned instead.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <returns>
    /// A valid <see cref="CultureInfo"/> obtained from the activity's <see cref="Activity.Locale"/> property.
    /// </returns>
    public static CultureInfo GetCultureInfoFromActivity(this IActivity activity)
    {
        return GetCultureFromLocale(activity.Locale);
    }

    /// <summary>
    /// Gets a <see cref="CultureInfo"/> from a message activity's <see cref="IActivity.Locale"/> property.
    /// If, for any reason, the property is <see langword="null"/> or empty, then the culture
    /// from <see cref="CultureInfo.CurrentCulture"/> is returned instead.
    /// </summary>
    /// <param name="messageActivity">The message activity.</param>
    /// <returns>
    /// A valid <see cref="CultureInfo"/> obtained from the activity's <see cref="IActivity.Locale"/> property.
    /// </returns>
    public static CultureInfo GetCultureInfoFromMessageActivity(this IMessageActivity messageActivity)
    {
        return GetCultureFromLocale(messageActivity.Locale);
    }

    private static CultureInfo GetCultureFromLocale(string locale)
    {
        // IMPORTANT - Under the hood, agents never set 'CurrentUICulture', they work only with 'CurrentCulture'.
        // For more info, review the implementation of the abstract class 'ChannelAdapter'.
        return string.IsNullOrWhiteSpace(locale) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(locale);
    }
}
