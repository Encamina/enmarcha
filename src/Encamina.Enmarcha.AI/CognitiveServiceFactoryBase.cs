using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.AI.Abstractions;

using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.AI;

/// <summary>
/// Base class for cognitive services factories.
/// </summary>
/// <typeparam name="TCognitiveServiceBase">The type of cognitive service.</typeparam>
/// <typeparam name="TCognitiveServiceOptions">The type of options for the cognitive service.</typeparam>
public class CognitiveServiceFactoryBase<TCognitiveServiceBase, TCognitiveServiceOptions> : ICognitiveServiceFactory<TCognitiveServiceBase>
    where TCognitiveServiceBase : CognitiveServiceBase<TCognitiveServiceOptions>
    where TCognitiveServiceOptions : CognitiveServiceOptionsBase
{
    private readonly IDictionary<string, TCognitiveServiceBase> cognitiveServices = new Dictionary<string, TCognitiveServiceBase>();

    /// <summary>
    /// Initializes a new instance of the <see cref="CognitiveServiceFactoryBase{TCognitiveServiceBase, TCognitiveServiceOptions}"/> class.
    /// </summary>
    /// <param name="configurations">The configurations for the cognitive services.</param>
    /// <param name="factory">A factory to create instances of the cognitive serivces.</param>
    protected CognitiveServiceFactoryBase(IOptions<ICognitiveServiceConfigurationsBase<TCognitiveServiceOptions>> configurations, Func<TCognitiveServiceOptions, TCognitiveServiceBase> factory)
    {
        Guard.IsNotNull(configurations);
        Guard.IsNotNull(configurations.Value);
        Guard.IsNotNull(configurations.Value.CognitiveServiceOptions);

        foreach (var options in configurations.Value.CognitiveServiceOptions)
        {
            cognitiveServices[options.Name.ToUpperInvariant()] = factory(options);
        }
    }

    /// <inheritdoc/>
    public virtual TCognitiveServiceBase GetByName(string cognitiveServiceName, bool throwIfNotFound)
    {
        var key = cognitiveServiceName.ToUpperInvariant();

        if (cognitiveServices.ContainsKey(key))
        {
            return cognitiveServices[key];
        }

        if (throwIfNotFound)
        {
            throw new ArgumentException(string.Format(Resources.ExceptionMessages.InvalidCognitiveServiceName, cognitiveServiceName), nameof(cognitiveServiceName));
        }

        return null;
    }

    /// <inheritdoc/>
    public TCognitiveServiceBase GetByName(string cognitiveServiceName) => GetByName(cognitiveServiceName, true);
}
