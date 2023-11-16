using Microsoft.Extensions.Configuration;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension helper methods for an <see cref="IConfiguration"/>.
/// </summary>
public static class IConfigurationExtensions
{
    /// <summary>
    /// Checks that two instances of <see cref="IConfiguration"/> are the same configurations in terms of their settings,
    /// regardless if they are or not the same instance.
    /// </summary>
    /// <remarks>This method might be usefull for testing scenarios.</remarks>
    /// <param name="first">First instance of <see cref="IConfiguration"/> to check.</param>
    /// <param name="actualConfiguration">Second instance of <see cref="IConfiguration"/> to check.</param>
    /// <returns>
    /// Returns <see langword="true"/> if both instances of <see cref="IConfiguration"/> are the same in terms of their settings; otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool IsSame(this IConfiguration first, IConfiguration actualConfiguration)
    {
        return IsSame(first, actualConfiguration.AsEnumerable());
    }

    /// <summary>
    /// Checks that an instance of <see cref="IConfiguration"/> contains the same settings as a given dictionary.
    /// </summary>
    /// <remarks>
    /// Under the hood, an instance of <see cref="IConfiguration"/> is like a dictionary. This method might be usefull for testing scenarios.
    /// </remarks>
    /// <param name="configuration">The instance of <see cref="IConfiguration"/> to check.</param>
    /// <param name="settings">The instance of a dictionary with the settings to check the configuration.</param>
    /// <returns>
    /// Returns <see langword="true"/> if the instance of <see cref="IConfiguration"/> has as settings the values from the given dictionary; otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool IsSame(this IConfiguration configuration, IDictionary<string, string> settings)
    {
        return IsSame(configuration, settings.AsEnumerable());
    }

    private static bool IsSame(IConfiguration expectedConfiguration, IEnumerable<KeyValuePair<string, string>> settings)
    {
        foreach (var setting in settings)
        {
            if (!expectedConfiguration[setting.Key].Equals(setting.Value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
