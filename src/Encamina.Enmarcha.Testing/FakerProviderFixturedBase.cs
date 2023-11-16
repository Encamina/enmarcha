using System.Diagnostics.CodeAnalysis;

namespace Encamina.Enmarcha.Testing;

/// <summary>
/// Base abstract class for faker provider fixtures for tests.
/// </summary>
[SuppressMessage("Minor Code Smell", "S1694:An abstract class should have both abstract and concrete methods", Justification = "It's the Architecture's intent that this class must be inherited!")]
public abstract class FakerProviderFixturedBase : IFakerProviderFixture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakerProviderFixturedBase"/> class.
    /// </summary>
    /// <param name="fakerProvider">The faker provider.</param>
    protected FakerProviderFixturedBase(FakerProvider fakerProvider)
    {
        FakerProvider = fakerProvider;
    }

    /// <inheritdoc/>
    public FakerProvider FakerProvider { get; }
}
