using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.LanguagesDetection.Azure;

internal record TranslatorLanguageDetectionServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Gets minimum confidence threshold score for language detection, value ranges from 0 to 1.
    /// </summary>
    public double? ConfidenceThreshold { get; init; }

    /// <summary>
    /// Gets the Azure region name of the translator resource.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string RegionName { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether any language detection result should only include languages that can be translated. Default is <see langword="true"/>.
    /// </summary>
    public bool DetectOnlyTranslatableLanguages { get; set; } = true;
}
