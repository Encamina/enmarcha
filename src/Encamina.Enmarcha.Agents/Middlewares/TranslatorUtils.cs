using System.Globalization;
using System.Text.Json;

using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Middlewares;

/// <summary>
/// General utilitarian methods for translation operations usually required or common to translation middlewares.
/// </summary>
internal static class TranslatorUtils
{
    /// <summary>
    /// Gets a translation.
    /// </summary>
    /// <param name="translations">Current collection of translations.</param>
    /// <param name="language">Expected translation language.</param>
    /// <param name="default">Default text if no translation is found. Defaults as <see cref="string.Empty"/>.</param>
    /// <returns>
    /// The translated text in the given <paramref name="language"/> or the given default value from <paramref name="default"/>.
    /// </returns>
    internal static string GetTranslation(IDictionary<string, string>? translations, CultureInfo language, string @default = @"")
    {
        return translations != null && (translations.TryGetValue(language.Name, out var value) || translations.TryGetValue(language.Parent.Name, out value)) ? value : @default;
    }

    /// <summary>
    /// Translates message type activities.
    /// </summary>
    /// <param name="translationService">A valid instance of a <see cref="ITextTranslationService">translation service</see>.</param>
    /// <param name="activities">Collection of message type activities.</param>
    /// <param name="fromLanguage">The language to translate from.</param>
    /// <param name="toLanguage">The language to translate to.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous error handling operation.</returns>
    internal static async Task TranslateMessagesAsync(ITextTranslationService translationService, IList<Activity> activities, CultureInfo? fromLanguage, CultureInfo toLanguage, CancellationToken cancellationToken)
    {
        var messageActivities = activities?.Where(a => a.Type == ActivityTypes.Message);

        if (messageActivities?.Any() ?? false)
        {
            // Use an index as translation identifier to keep track of translatable items, which could be more than just the message, since it might also include attachments and other elements.
            var index = 0;
            var translatableItems = new Dictionary<string, string>();

            foreach (var messageActivity in messageActivities)
            {
                translatableItems.Add(index.ToString(), messageActivity.Text ?? string.Empty);
                index = GetAttachmentsTranslatableItems(index, messageActivity.Attachments, translatableItems);
                index = GetSuggestedActionsTranslatableItems(index, messageActivity.SuggestedActions, translatableItems);
            }

            var translationResults = await translationService.TranslateAsync(new TextTranslationRequest()
            {
                FromLanguage = fromLanguage,
                Texts = translatableItems,
                ToLanguages = [toLanguage],
            }, cancellationToken);

            index = 0;

            foreach (var messageActivity in messageActivities)
            {
                var translationResult = translationResults.TextTranslations.ElementAt(index);
                messageActivity.Text = GetTranslation(translationResult.Translations, toLanguage);
                index = SetAttachmentsTranslatableItems(index, toLanguage, translationResults.TextTranslations, messageActivity.Attachments);
                index = SetSuggestedActionsTranslatableItems(index, toLanguage, translationResults.TextTranslations, messageActivity.SuggestedActions);
            }
        }
    }

    private static int GetAttachmentsTranslatableItems(int index, IList<Attachment> attachments, IDictionary<string, string> translatableItems)
    {
        if (attachments != null)
        {
            foreach (var attachment in attachments.Select(a => a.Content))
            {
                switch (attachment)
                {
                    case HeroCard heroCard:
                        index = GetHeroCardTranslatableItems(index, heroCard, translatableItems);
                        break;

                    case JsonElement:
                        break;

                    default:
                        throw new NotSupportedException(string.Format(Resources.ExceptionMessages.TranslationSupportNotImplementedForAttachmentType, attachment));
                }
            }
        }

        return index;
    }

    private static int GetEmptyActionTypeTranslatableItems(int index, CardAction action, IDictionary<string, string> translatableItems)
    {
        translatableItems.Add((++index).ToString(), action.Title ?? string.Empty);

        return index;
    }

    private static int GetHeroCardTranslatableItems(int index, HeroCard card, IDictionary<string, string> translatableItems)
    {
        translatableItems.Add((++index).ToString(), card.Text ?? string.Empty);
        translatableItems.Add((++index).ToString(), card.Title ?? string.Empty);
        translatableItems.Add((++index).ToString(), card.Subtitle ?? string.Empty);

        return index;
    }

    private static int GetMessageBackActionTypeTranslatableItems(int index, CardAction action, IDictionary<string, string> translatableItems)
    {
        translatableItems.Add((++index).ToString(), action.Title ?? string.Empty);
        translatableItems.Add((++index).ToString(), action.DisplayText ?? string.Empty);

        return index;
    }

    private static int GetSuggestedActionsTranslatableItems(int index, SuggestedActions suggestedActions, IDictionary<string, string> translatableItems)
    {
        if (suggestedActions != null)
        {
            foreach (var action in suggestedActions.Actions)
            {
                switch (action.Type)
                {
                    case ActionTypes.MessageBack:
                        index = GetMessageBackActionTypeTranslatableItems(index, action, translatableItems);
                        break;

                    case @"":
                    case null:
                        index = GetEmptyActionTypeTranslatableItems(index, action, translatableItems);
                        break;

                    default:
                        throw new NotSupportedException(string.Format(Resources.ExceptionMessages.TranslationSupportNotImplementedForActionType, action.Type));
                }
            }
        }

        return index;
    }

    private static int SetAttachmentsTranslatableItems(int index, CultureInfo cultureLanguage, IEnumerable<ITextTranslation> textTranslations, IList<Attachment> attachments)
    {
        if (attachments != null)
        {
            foreach (var attachment in attachments.Select(a => a.Content))
            {
                switch (attachment)
                {
                    case HeroCard heroCard:
                        index = SetHeroCardTranslatableItems(index, cultureLanguage, heroCard, textTranslations);
                        break;

                    case JsonElement:
                        break;

                    default:
                        throw new NotSupportedException(string.Format(Resources.ExceptionMessages.TranslationSupportNotImplementedForAttachmentType, attachment));
                }
            }
        }

        return index;
    }

    private static int SetEmptyActionTypeTranslatableItems(int index, CultureInfo cultureLanguage, CardAction action, IEnumerable<ITextTranslation> textTranslations)
    {
        action.Title = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);

        return index;
    }

    private static int SetHeroCardTranslatableItems(int index, CultureInfo cultureLanguage, HeroCard card, IEnumerable<ITextTranslation> textTranslations)
    {
        card.Text = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);
        card.Title = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);
        card.Subtitle = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);

        return index;
    }

    private static int SetMessageBackActionTypeTranslatableItems(int index, CultureInfo cultureLanguage, CardAction action, IEnumerable<ITextTranslation> textTranslations)
    {
        action.Title = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);
        action.DisplayText = GetTranslation(textTranslations.ElementAt(++index).Translations, cultureLanguage);

        return index;
    }

    private static int SetSuggestedActionsTranslatableItems(int index, CultureInfo cultureLanguage, IEnumerable<ITextTranslation> textTranslations, SuggestedActions suggestedActions)
    {
        if (suggestedActions != null)
        {
            foreach (var action in suggestedActions.Actions)
            {
                switch (action.Type)
                {
                    case ActionTypes.MessageBack:
                        index = SetMessageBackActionTypeTranslatableItems(index, cultureLanguage, action, textTranslations);
                        break;

                    case @"":
                    case null:
                        index = SetEmptyActionTypeTranslatableItems(index, cultureLanguage, action, textTranslations);
                        break;

                    default:
                        throw new NotSupportedException(string.Format(Resources.ExceptionMessages.TranslationSupportNotImplementedForActionType, action.Type));
                }
            }
        }

        return index;
    }
}
