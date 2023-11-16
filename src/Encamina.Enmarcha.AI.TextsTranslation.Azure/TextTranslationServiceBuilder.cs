using Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.AI.TextsTranslation.Azure;

/// <summary>
/// Allows fine grained configuration of Text Translation services.
/// </summary>
internal sealed class TextTranslationServiceBuilder : ITextTranslationServiceBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextTranslationServiceBuilder"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    public TextTranslationServiceBuilder(IServiceCollection services)
    {
        CommunityToolkit.Diagnostics.Guard.IsNotNull(services);
        Services = services;
    }

    /// <inheritdoc />
    public IServiceCollection Services { get; }
}
