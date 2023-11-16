using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.QuestionsAnswering.Azure;

/// <summary>
/// A factory that provides valid instances of question answering (cognitive) services.
/// </summary>
internal class QuestionAnsweringServiceFactory : CognitiveServiceFactoryBase<QuestionAnsweringService, QuestionAnsweringServiceOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionAnsweringServiceFactory"/> class.
    /// </summary>
    /// <param name="configurations">Configurations for cognitive services.</param>
    public QuestionAnsweringServiceFactory(IOptions<QuestionAnsweringConfigurations> configurations)
        : base(configurations, options => new QuestionAnsweringService(options))
    {
    }
}
