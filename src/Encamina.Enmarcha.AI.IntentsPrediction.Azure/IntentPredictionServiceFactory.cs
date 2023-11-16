using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI.IntentsPrediction.Azure;

/// <summary>
/// A factory that provides valid instances of intent prediction (cognitive) services.
/// </summary>
internal class IntentPredictionServiceFactory : CognitiveServiceFactoryBase<IntentPredictionService, IntentPredictionServiceOptions>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntentPredictionServiceFactory"/> class.
    /// </summary>
    /// <param name="configurations">Configurations for cognitive services.</param>
    public IntentPredictionServiceFactory(IOptions<IntentPredictionConfigurations> configurations)
       : base(configurations, options => new IntentPredictionService(options))
    {
    }
}
