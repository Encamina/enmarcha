using Encamina.Enmarcha.Bot.Abstractions.Activities;

using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Cards;

/// <summary>
/// Contains utility methods to create various <see cref="CardAction"/>.
/// </summary>
public static class CardActionFactory
{
    /// <summary>
    /// Creates a <see cref="CardAction"/> without action and only text.
    /// </summary>
    /// <param name="text">The text for the action.</param>
    /// <returns>A new <see cref="CardAction"/> without action and only text.</returns>
    public static CardAction EmptyAction(string text)
    {
        return new CardAction()
        {
            Title = text,
            Type = null,
        };
    }

    /// <summary>
    /// Creates a <see cref="CardAction"/> that can be used to message back the bot with some values.
    /// </summary>
    /// <typeparam name="TValue">
    /// The value to store in the <see cref="CardAction"/>. <b>It must injerit from <see cref="ActivityValueBase"/> to ensure proper work of the activity processor's engine.</b>
    /// </typeparam>
    /// <param name="value">
    /// The value to sent when the messages comes back to the bot. <b>It must injerit from <see cref="ActivityValueBase"/> to ensure proper work of the activity processor's engine.</b>
    /// </param>
    /// <param name="text">The text for this action.</param>
    /// <param name="imageUrl">An optional image URL which will appear on the button.</param>
    /// <returns>A new <see cref="CardAction"/> that can be used to message back the bot with some values.</returns>
    public static CardAction MessageBackAction<TValue>(TValue value, string text, string imageUrl) where TValue : ActivityValueBase
    {
        return new CardAction()
        {
            Image = imageUrl,
            Title = text,
            Type = ActionTypes.MessageBack,
            Value = value,
        };
    }

    /// <summary>
    /// Creates a <see cref="CardAction"/> that can be used to message back the bot with some values and also show in the chat feed a message as if it was send by the user.
    /// </summary>
    /// <typeparam name="TValue">
    /// The value to store in the <see cref="CardAction"/>. <b>It must injerit from <see cref="ActivityValueBase"/> to ensure proper work of the activity processor's engine.</b>
    /// </typeparam>
    /// <param name="value">
    /// The value to sent when the messages comes back to the bot. <b>It must injerit from <see cref="ActivityValueBase"/> to ensure proper work of the activity processor's engine.</b>
    /// </param>
    /// <param name="text">The text for this action.</param>
    /// <param name="imageUrl">An optional image URL which will appear on the button.</param>
    /// <param name="displayText">An optional text to display in the chat feed if the card action is used.</param>
    /// <returns>A new <see cref="CardAction"/> that can be used to message back the bot with some values.</returns>
    public static CardAction MessageBackWithDisplayTextAction<TValue>(TValue value, string text, string imageUrl, string displayText) where TValue : ActivityValueBase
    {
        // This is a hack that allows creating a card action that messages back to the bot while also showing a message to the user as if the user has written it.
        var title = string.IsNullOrWhiteSpace(text) ? @" " : text;

        return new CardAction()
        {
            Image = imageUrl,
            Title = title,
            Type = ActionTypes.MessageBack,
            Value = value,
            Text = displayText,
            DisplayText = displayText,
        };
    }
}
