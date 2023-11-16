using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Testing;

/// <summary>
/// Collection of public constants that represents fixed string values that provide behavioral services.
/// </summary>
[SuppressMessage("Critical Code Smell",
                 "S2339:Public constant members should not be used",
                 Justification = "Members on this type are expected to be used with Attributes which requires constants (see error CS0182).")]
public static class MagicStrings
{
    /// <summary>
    /// Use this magic string to identify fixtures collections that includes a large set of test fixtures.
    /// </summary>
    /// <seealso href="https://xunit.net/docs/shared-context#:~:text=xUnit.net%20treats%20collection%20fixtures,the%20collection%20have%20finished%20running."/>
    public const string FixturesCollection = @"FixturesCollection";
}
