﻿using System.Globalization;

using Encamina.Enmarcha.Bot.Abstractions.Cards;

using Microsoft.Bot.Schema;

namespace Encamina.Enmarcha.Bot.Abstractions.Greetings;

/// <summary>
/// Represents localized options for greetings messages based on <see cref="HeroCard">hero cards</see>.
/// </summary>
public interface ILocalizedHeroCardGreetingsOptions
{
    /// <summary>
    /// Gets the default locale (i.e., language) for a <see cref="HeroCard">hero card</see> greetings message.
    /// </summary>
    public CultureInfo DefaultLocale { get; }

    /// <summary>
    /// Gets a dictionary of localizard options for <see cref="HeroCard">hero cards</see>.
    /// The <see cref="IDictionary{TKey, TValue}.Keys"/> are locale or language codes.
    /// </summary>
    public IDictionary<CultureInfo, IEnumerable<IHeroCardOptions>> LocalizedOptions { get; }
}
