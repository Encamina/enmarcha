using System.Globalization;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.Extensions;

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
    /// Returns <see langword="true"/> if the activity is an start activity, otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool IsStartActivity(this Activity activity)
    {
        return activity.ChannelId switch
        {
            Channels.Skype => activity.Type == ActivityTypes.ContactRelationUpdate && activity.Action == Add,
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
    public static CultureInfo GetCultureInfoFromActivity(this Activity activity)
    {
        return GetCultureFromLocale(activity.Locale);
    }

    /// <summary>
    /// Gets a <see cref="CultureInfo"/> from a message activity's <see cref="IMessageActivity.Locale"/> property.
    /// If, for any reason, the property is <see langword="null"/> or empty, then the culture
    /// from <see cref="CultureInfo.CurrentCulture"/> is returned instead.
    /// </summary>
    /// <param name="messageActivity">The message activity.</param>
    /// <returns>
    /// A valid <see cref="CultureInfo"/> obtained from the activity's <see cref="IMessageActivity.Locale"/> property.
    /// </returns>
    public static CultureInfo GetCultureInfoFromMessageActivity(this IMessageActivity messageActivity)
    {
        return GetCultureFromLocale(messageActivity.Locale);
    }

    private static CultureInfo GetCultureFromLocale(string locale)
    {
        // IMPORTANT - Under the hood, bots never set 'CurrentUICulture', they work only with 'CurrentCulture'.
        // For more info, review the implementation of the abstract class 'BotAdapter'.
        return string.IsNullOrWhiteSpace(locale) ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(locale);
    }
}
