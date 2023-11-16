using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// Extension methods to configure Text Translation services.
/// </summary>
public static class ITextTranslationServiceBuilderExtensions
{
    /// <summary>
    /// Sets the usage of a specific text translation normalizer.
    /// </summary>
    /// <typeparam name="TNormalizer">The type of the text translation normalizer.</typeparam>
    /// <param name="builder">The <see cref="ITextTranslationServiceBuilder"/> instance this method extends.</param>
    /// <returns>The <see cref="ITextTranslationServiceBuilder"/> so that additional calls can be chained.</returns>
    public static ITextTranslationServiceBuilder UseNormalizer<TNormalizer>(this ITextTranslationServiceBuilder builder)
        where TNormalizer : class, ITextTranslationNormalizer
    {
        builder.Services.AddSingleton<TNormalizer>();
        builder.Services.AddSingleton<ITextTranslationNormalizer, TNormalizer>();

        return builder;
    }
}
