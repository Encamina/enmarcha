using System.ComponentModel.DataAnnotations;

using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.TextsTranslation.Azure;

internal record TextTranslationServiceOptions : CognitiveServiceOptionsBase
{
    /// <summary>
    /// Gets the Azure region name of the translator resource.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public string RegionName { get; init; }
}
