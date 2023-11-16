using CommunityToolkit.Diagnostics;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Abstractions.Extensions;

/// <summary>
/// Utilitarian extension methods for <see cref="LanguageDetectionRequest"/>.
/// </summary>
public static class LanguageDetectionRequestExtensions
{
    /// <summary>
    /// Validates if a <see cref="LanguageDetectionRequest"/> is valid or not.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <exception cref="ArgumentException">
    /// If any of the components of the <paramref name="request"/> is invalid or unexpected.
    /// </exception>
    public static void Validate(this LanguageDetectionRequest request)
    {
        Guard.IsNotNull(request);
        Guard.IsNotNull(request.Text);

        if (request.Text.Any(t => string.IsNullOrWhiteSpace(t.Id)))
        {
            throw new ArgumentException(Resources.ExceptionMessages.RequestTextWithInvalidIdentifier);
        }

        if (request.Text.Select(t => t.Id).Distinct().Count() < request.Text.Count())
        {
            throw new ArgumentException(Resources.ExceptionMessages.RequestTextWithRepeatedIdentifier);
        }

        if (request.Text.Any(t => string.IsNullOrWhiteSpace(t.Value)))
        {
            throw new ArgumentException(Resources.ExceptionMessages.RequestTextWithInvalidValue);
        }
    }
}
