using Encamina.Enmarcha.AI.Abstractions;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure;

/// <summary>
/// Configurations for question answering services.
/// </summary>
internal record QuestionAnsweringConfigurations : ICognitiveServiceConfigurationsBase<QuestionAnsweringServiceOptions>
{
    /// <summary>
    /// Gets the collection of specific question answering service options in this configuration.
    /// </summary>
    public IReadOnlyList<QuestionAnsweringServiceOptions> QuestionAnsweringOptions { get; init; }

    /// <inheritdoc/>
    public IReadOnlyList<QuestionAnsweringServiceOptions> CognitiveServiceOptions => QuestionAnsweringOptions;
}
