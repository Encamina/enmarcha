using Microsoft.Agents.Core.Models;

namespace Encamina.Enmarcha.Agents.Abstractions.Cards;

/// <summary>
/// Represents common options to create a <see cref="HeroCard"/>.
/// </summary>
public interface IHeroCardOptions
{
    /// <summary>
    /// Gets the hero card's title.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets the hero card's subtitle.
    /// </summary>
    string Subtitle { get; }

    /// <summary>
    /// Gets the hero card's text.
    /// </summary>
    string Text { get; }

    /// <summary>
    /// Gets the hero card's collection of images.
    /// </summary>
    IList<CardImage> Images { get; }
}
