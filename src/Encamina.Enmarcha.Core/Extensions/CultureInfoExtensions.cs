using System.Globalization;

namespace Encamina.Enmarcha.Core.Extensions;

/// <summary>
/// Extension methods for <see cref="CultureInfo"/> instances.
/// </summary>
public static class CultureInfoExtensions
{
    /// <summary>
    /// Determines if this <see cref="CultureInfo"/> matches another or any of its parent cultures.
    /// </summary>
    /// <param name="cultureInfo">The current <see cref="CultureInfo"/>.</param>
    /// <param name="matchCandidate">The match candidate <see cref="CultureInfo"/>.</param>
    /// <returns>
    /// Returns <see langword="true"/> if this <see cref="CultureInfo"/> matches with <paramref name="matchCandidate"/>, otherwise returns <see langword="false"/>.
    /// </returns>
    public static bool MatchesWith(this CultureInfo cultureInfo, CultureInfo matchCandidate)
    {
        return (CultureInfo.InvariantCulture.Equals(cultureInfo) && CultureInfo.InvariantCulture.Equals(matchCandidate)) ||
               ((!CultureInfo.InvariantCulture.Equals(cultureInfo) || CultureInfo.InvariantCulture.Equals(matchCandidate)) &&
                (cultureInfo.Equals(matchCandidate) || cultureInfo.Parent.MatchesWith(matchCandidate)));
    }
}
