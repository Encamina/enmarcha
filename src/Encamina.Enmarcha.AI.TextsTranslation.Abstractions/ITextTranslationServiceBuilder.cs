using Microsoft.Extensions.DependencyInjection;

namespace Encamina.Enmarcha.AI.TextsTranslation.Abstractions;

/// <summary>
/// An interface for configuring Text Translation services.
/// </summary>
public interface ITextTranslationServiceBuilder
{
    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> where Text Translation services are configured.
    /// </summary>
    IServiceCollection Services { get; }
}
