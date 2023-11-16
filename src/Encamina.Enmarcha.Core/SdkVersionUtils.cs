using System.Globalization;
using System.Reflection;

namespace Encamina.Enmarcha.Core;

/// <summary>
/// Utility class for the version information of the current assembly.
/// </summary>
public static class SdkVersionUtils
{
    /// <summary>
    /// Gets the SDK version with the specified version prefix.
    /// </summary>
    /// <param name="versionPrefix">The version prefix to use.</param>
    /// <param name="assembly">The assembly to get the version from.</param>
    /// <returns>The SDK version.</returns>
    public static string GetSdkVersion(string versionPrefix, Assembly assembly)
    {
        var versionStr = assembly.GetCustomAttributes<AssemblyFileVersionAttribute>().First().Version;
        var version = new Version(versionStr);
        return (versionPrefix ?? string.Empty) + version.ToString(3) + "-" + version.Revision.ToString(CultureInfo.InvariantCulture);
    }
}
